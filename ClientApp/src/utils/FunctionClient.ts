import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from "@microsoft/signalr";
import { WebRequest, WebResponse } from "../models/types";
import { ErrorCode } from "../models/ErrorCode";
import { AlertColor } from "@mui/material";

type AlertCallback = (message: string, severity: AlertColor) => void;
type ReconnectCallback = (connectionId?: string) => void;

export class FunctionClient {
    private connection: HubConnection;
    private static instance: FunctionClient;
    private onAlertCallback?: AlertCallback;
    private onReconnectCallback?: ReconnectCallback;

    private constructor(hubUrl: string) {
        this.connection = new HubConnectionBuilder()
            .withUrl(hubUrl)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    if (retryContext.previousRetryCount >= 5) {
                        this.onAlertCallback?.("Cannot connect to app device. Try to rerun application or contact with developer", "error");
                        return null;
                    }
                    return Math.min(retryContext.previousRetryCount * 1000, 30000);
                }
            })
            .build();

        this.setupEventListeners();
    }

    private setupEventListeners() {
        this.connection.onreconnecting((error) => {
            console.warn("SignalR Reconnecting:", error);
            this.onAlertCallback?.("Connection lost. Trying to reconnect...", "warning");
        });

        this.connection.onreconnected((connectionId) => {
            console.log("SignalR Reconnected:", connectionId);
            this.onAlertCallback?.("Reconnected", "success");
            if (this.onReconnectCallback) {
                this.onReconnectCallback(connectionId);
            }
        });

        this.connection.onclose((error) => {
            if (error) {
                this.onAlertCallback?.("Connection closed due to app error. Please run application again.", "error");
            }
        });
    }

    public setOnReconnectHandler(callback: ReconnectCallback) {
        this.onReconnectCallback = callback;
    }

    public static getInstance(url?: string): FunctionClient {
        if (!FunctionClient.instance) {
            if (!url) throw new Error("SignalR: URL is required for the first initialization!");
            FunctionClient.instance = new FunctionClient(url);
        }
        return FunctionClient.instance;
    }

    public setOnAlertHandler(callback: AlertCallback) {
        this.onAlertCallback = callback;
    }

    async start(count: number = 0) {
        if (this.connection.state !== HubConnectionState.Disconnected) return;

        try {
            await this.connection.start();
            if (count > 1) this.onAlertCallback?.("Reconnected", "success")
        } catch (err) {
            this.onAlertCallback?.("Cannot get device app. Retry in 10 seconds", "warning");
            setTimeout(() => this.start(count), 10000);
        }
    }

    async waitForConnection(timeoutMs = 15000): Promise<void> {
        const start = Date.now();

        while (this.connection.state !== HubConnectionState.Connected) {
            if (Date.now() - start > timeoutMs) {
                throw new Error("SignalR connection timeout");
            }
            await new Promise(res => setTimeout(res, 100));
        }
    }

    async callFunction<TBody = any, TResult = any>(
        functionName: string,
        body?: TBody
    ): Promise<WebResponse<TResult>> {
        const request: WebRequest = {
            requestId: crypto.randomUUID(),
            functionName,
            body: body ? JSON.stringify(body) : "",
        };

        if (this.connection.state !== HubConnectionState.Connected) {
            return {
                requestId: request.requestId,
                statusCode: 503,
                body: null,
                errorCode: ErrorCode.Unknown,
                details: "Service unavailable. Not connected to server.",
            };
        }

        try {
            return await this.connection.invoke<WebResponse<TResult>>("Call", request);
        } catch (err: any) {
            return {
                requestId: request.requestId,
                statusCode: 500,
                body: null,
                errorCode: ErrorCode.Unknown,
                details: err.message,
            };
        }
    }

    on<TEvent = any>(eventName: string, handler: (data: TEvent) => void) {
        this.connection.on(eventName, handler);
    }

    off(eventName: string, handler?: (data: any) => void) {
        if (handler) this.connection.off(eventName, handler);
        else this.connection.off(eventName);
    }

    async joinIrcConnection(connectionId: string) {
        await this.waitForConnection();
        await this.connection.invoke("JoinToConnection", connectionId);
    }

    async leaveIrcConnection(connectionId: string) {
        if (this.connection.state !== HubConnectionState.Connected) return;
        await this.connection.invoke("LeaveConnection", connectionId);
    }
}

export const signalRClient = FunctionClient.getInstance("http://localhost:5727/hub");