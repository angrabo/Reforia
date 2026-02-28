import { useState, ChangeEvent, FormEvent } from "react";
import { Box, TextField, Button, Stack, Typography } from "@mui/material";
import { useChat } from "../../Context/ChatContext.tsx";
import { ModalBase } from "../modals/ModalBase.tsx";

interface AddChatModalProps {
    open: boolean;
    onClose: () => void;
}

const AddChatModal = ({ open, onClose }: AddChatModalProps) => {
    const { joinChannel } = useChat();
    const [form, setForm] = useState({ id: "", tournament: "", stage: "" });

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        if (!form.id) return;

        try {
            await joinChannel(form.id, form.tournament, form.stage);
            setForm({ id: "", tournament: "", stage: "" });
            onClose();
        } catch (error) {
            console.error("Failed to join channel:", error);
        }
    };

    return (
        <ModalBase
            open={open}
            onClose={onClose}
            title="Add Channel"
            maxWidth={450}
        >
            <Box
                component="form"
                onSubmit={handleSubmit}
                sx={{ mt: 1 }}
                autoComplete="off"
            >
                <Stack spacing={3}>
                    <Typography variant="body2" sx={{ color: "rgba(255, 255, 255, 0.5)" }}>
                        Enter the channel ID to join the conversation.
                    </Typography>

                    <TextField
                        name="id"
                        label="Channel ID"
                        placeholder="#mp_1234567"
                        fullWidth
                        required
                        value={form.id}
                        onChange={handleChange}
                        autoFocus
                    />

                    <Button
                        disabled
                        variant="outlined"
                        sx={{
                            fontWeight: '500',
                            color: "rgba(255, 255, 255, 0.3)",
                            borderColor: "rgba(255, 255, 255, 0.1) !important",
                            textTransform: 'none',
                            "&.Mui-disabled": {
                                color: "rgba(255, 255, 255, 0.2)",
                            }
                        }}
                    >
                        Import tournament (Coming soon!)
                    </Button>

                    <Button
                        type="submit"
                        variant="contained"
                        disabled={!form.id}
                    >
                        Add Channel
                    </Button>
                </Stack>
            </Box>
        </ModalBase>
    );
};

export default AddChatModal;