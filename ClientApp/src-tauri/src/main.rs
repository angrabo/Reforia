// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use std::{
    process::{Child, Command},
    sync::Mutex,
};

use tauri::{AppHandle, Manager, RunEvent, WindowEvent};
use std::os::windows::process::CommandExt;

struct BackendProcess(Mutex<Option<Child>>);

fn start_backend(app: &AppHandle) -> Child {
    let resource_path = app
        .path()
        .resolve(
            "binaries/Reforia.DeviceApp.exe",
            tauri::path::BaseDirectory::Resource,
        )
        .expect("Backend not found");

    Command::new(resource_path)
        .spawn()
        .expect("Cannot run device app")
}

fn stop_backend(state: &BackendProcess) {
    let mut lock = state.0.lock().unwrap();
    if let Some(mut child) = lock.take() {
        let _ = child.kill();
        let _ = child.wait();
    }
}

#[tauri::command]
fn finish_loading(app: AppHandle) {
    if let Some(splash) = app.get_webview_window("splash") {
        let _ = splash.close();
    }

    if let Some(main) = app.get_webview_window("main") {
        let _ = main.show();
        let _ = main.set_focus();
    }
}

fn kill_old_backend() {
    #[cfg(target_os = "windows")]
    {
        #[cfg(not(debug_assertions))]
        {
            println!("Killing old backend processes");
            const CREATE_NO_WINDOW: u32 = 0x08000000;

            let _ = Command::new("taskkill")
                .args(&["/F", "/IM", "Reforia.DeviceApp.exe", "/T"])
                .creation_flags(CREATE_NO_WINDOW)
                .status();
        }

        #[cfg(debug_assertions)]
        {
            println!("Debug mode: Skipping taskkill");
        }
    }
}
fn main() {
    kill_old_backend();
    tauri::Builder::default()
        .plugin(tauri_plugin_updater::Builder::new().build())
        .plugin(tauri_plugin_opener::init())
        .manage(BackendProcess(Mutex::new(None)))
        .invoke_handler(tauri::generate_handler![finish_loading])
        .setup(|app| {
            let handle = app.handle().clone();

            let child = start_backend(&handle);
            *app.state::<BackendProcess>().0.lock().unwrap() = Some(child);

            if let Some(splash) = app.get_webview_window("splash") {
                let splash_handle = handle.clone();
                splash.on_window_event(move |event| {
                    if let WindowEvent::Destroyed = event {
                        let main_win = splash_handle.get_webview_window("main");
                        let is_main_visible = main_win
                            .map(|w| w.is_visible().unwrap_or(false))
                            .unwrap_or(false);

                        if !is_main_visible {
                            stop_backend(splash_handle.state::<BackendProcess>().inner());
                            splash_handle.exit(0);
                        }
                    }
                });
            }

            Ok(())
        })
        .build(tauri::generate_context!())
        .expect("error while building tauri application")
        .run(|_app_handle, event| match event {
            RunEvent::ExitRequested { .. } | RunEvent::Exit => {
                stop_backend(_app_handle.state::<BackendProcess>().inner());
            }
            _ => {}
        });
}
