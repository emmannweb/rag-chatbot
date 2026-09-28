import { useState, useRef, useEffect } from "react";
import {
  Box,
  Paper,
  Typography,
  TextField,
  IconButton,
  Avatar,
  CircularProgress,
  Tooltip,
  Fade,
} from "@mui/material";
import { Send, Bot, User, Sparkles, MessageSquare, X } from "lucide-react";

// Configure your backend API endpoint here
const API_URL = "http://localhost:5156/api/v1/rag/chats";

export const ChatbotStream = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      sender: "bot",
      text: "Hello! I am your AI assistant. Ask me anything about your uploaded company policies.",
      timestamp: new Date().toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
      }),
    },
  ]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const messagesEndRef = useRef(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    if (isOpen) {
      scrollToBottom();
    }
  }, [messages, isOpen]);

  const handleSend = async (e) => {
    e.preventDefault();
    if (!input.trim() || loading) return;

    const userMessage = {
      sender: "user",
      text: input,
      timestamp: new Date().toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
      }),
    };

    setMessages((prev) => [...prev, userMessage]);
    const currentQuery = input;
    setInput("");
    setLoading(true);

    try {
      const response = await fetch(API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ question: currentQuery }),
      });

      if (!response.ok) {
        throw new Error("Failed to fetch response from server.");
      }

      // 1. Stop loading spinner once the stream connection opens
      setLoading(false);

      // 2. Append an empty placeholder bot message that we will dynamically fill chunk-by-chunk
      setMessages((prev) => [
        ...prev,
        {
          sender: "bot",
          text: "",
          timestamp: new Date().toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          }),
        },
      ]);

      // 3. Set up the stream reader
      const reader = response.body.getReader();
      const decoder = new TextDecoder();
      let accumulatedText = "";

      while (true) {
        const { value, done } = await reader.read();
        if (done) break;

        const chunk = decoder.decode(value, { stream: true });
        accumulatedText += chunk;

        // 4. Update only the last message progressively with the new text chunk
        setMessages((prev) => {
          const newMessages = [...prev];
          newMessages[newMessages.length - 1] = {
            ...newMessages[newMessages.length - 1],
            text: accumulatedText,
          };
          return newMessages;
        });
      }
    } catch (error) {
      console.error("Chat error:", error);
      setLoading(false);
      setMessages((prev) => [
        ...prev,
        {
          sender: "bot",
          text: "⚠️ Sorry, I encountered an error connecting to the backend system. Please make sure the service is running.",
          timestamp: new Date().toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          }),
        },
      ]);
    }
  };

  return (
    <Box sx={{ position: "fixed", bottom: 24, right: 24, zIndex: 1300 }}>
      {/* Floating Action Button to toggle chat */}
      {!isOpen && (
        <Tooltip title="Chat with AI Assistant" placement="left">
          <IconButton
            onClick={() => setIsOpen(true)}
            sx={{
              backgroundColor: "primary.main",
              color: "white",
              width: 60,
              height: 60,
              boxShadow: "0px 4px 20px rgba(0,0,0,0.15)",
              "&:hover": {
                backgroundColor: "primary.dark",
                transform: "scale(1.05)",
              },
              transition: "all 0.2s ease-in-out",
            }}
          >
            <Sparkles size={28} />
          </IconButton>
        </Tooltip>
      )}

      {/* Chat Window Panel */}
      <Fade in={isOpen}>
        <Paper
          elevation={6}
          sx={{
            display: isOpen ? "flex" : "none",
            flexDirection: "column",
            width: { xs: "90vw", sm: 380 },
            height: 520,
            borderRadius: 4,
            overflow: "hidden",
            backgroundColor: "#f8fafc",
            border: "1px solid",
            borderColor: "divider",
            boxShadow: "0px 10px 30px rgba(0,0,0,0.12)",
          }}
        >
          {/* Header */}
          <Box
            sx={{
              p: 2,
              backgroundColor: "primary.main",
              color: "primary.contrastText",
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
              <Avatar sx={{ bgcolor: "primary.dark", width: 36, height: 36 }}>
                <Bot size={20} />
              </Avatar>
              <Box>
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: 600, lineHeight: 1.2 }}
                >
                  RAG Chatbot Assistant
                </Typography>
                <Typography variant="caption" sx={{ opacity: 0.85 }}>
                  Online • Powered by Ollama & pgvector
                </Typography>
              </Box>
            </Box>
            <IconButton
              size="small"
              onClick={() => setIsOpen(false)}
              sx={{
                color: "white",
                "&:hover": { backgroundColor: "rgba(255,255,255,0.1)" },
              }}
            >
              <X size={20} />
            </IconButton>
          </Box>

          {/* Messages Body */}
          <Box
            sx={{
              flex: 1,
              p: 2,
              overflowY: "auto",
              display: "flex",
              flexDirection: "column",
              gap: 2,
              backgroundColor: "#ffffff",
            }}
          >
            {messages.map((msg, index) => {
              const isBot = msg.sender === "bot";
              return (
                <Box
                  key={index}
                  sx={{
                    display: "flex",
                    alignItems: "flex-start",
                    gap: 1,
                    alignSelf: isBot ? "flex-start" : "flex-end",
                    maxWidth: "85%",
                  }}
                >
                  {isBot && (
                    <Avatar
                      sx={{
                        bgcolor: "grey.200",
                        color: "primary.main",
                        width: 30,
                        height: 30,
                      }}
                    >
                      <Bot size={16} />
                    </Avatar>
                  )}
                  <Box>
                    <Paper
                      elevation={0}
                      sx={{
                        p: 1.5,
                        borderRadius: 2,
                        backgroundColor: isBot ? "#f1f5f9" : "primary.main",
                        color: isBot ? "text.primary" : "primary.contrastText",
                        borderTopLeftRadius: isBot ? 0 : 2,
                        borderTopRightRadius: isBot ? 2 : 0,
                        wordBreak: "break-word",
                        fontSize: "0.9rem",
                        lineHeight: 1.5,
                        whiteSpace: "pre-wrap", // Preserves spacing and newlines from LLM
                      }}
                    >
                      {msg.text}
                    </Paper>
                    <Typography
                      variant="caption"
                      sx={{
                        display: "block",
                        mt: 0.5,
                        fontSize: "0.65rem",
                        color: "text.secondary",
                        textAlign: isBot ? "left" : "right",
                      }}
                    >
                      {msg.timestamp}
                    </Typography>
                  </Box>
                  {!isBot && (
                    <Avatar
                      sx={{
                        bgcolor: "primary.light",
                        color: "primary.contrastText",
                        width: 30,
                        height: 30,
                      }}
                    >
                      <User size={16} />
                    </Avatar>
                  )}
                </Box>
              );
            })}

            {loading && (
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  alignSelf: "flex-start",
                }}
              >
                <Avatar
                  sx={{
                    bgcolor: "grey.200",
                    color: "primary.main",
                    width: 30,
                    height: 30,
                  }}
                >
                  <Bot size={16} />
                </Avatar>
                <Paper
                  elevation={0}
                  sx={{
                    p: 2,
                    borderRadius: 2,
                    backgroundColor: "#f1f5f9",
                    display: "flex",
                    alignItems: "center",
                    gap: 1,
                  }}
                >
                  <CircularProgress size={16} thickness={5} />
                  <Typography variant="body2" color="text.secondary">
                    Thinking and searching records...
                  </Typography>
                </Paper>
              </Box>
            )}
            <div ref={messagesEndRef} />
          </Box>

          {/* Input Footer */}
          <Box
            component="form"
            onSubmit={handleSend}
            sx={{
              p: 1.5,
              backgroundColor: "#f8fafc",
              borderTop: "1px solid",
              borderColor: "divider",
              display: "flex",
              alignItems: "center",
              gap: 1,
            }}
          >
            <TextField
              fullWidth
              size="small"
              placeholder="Ask a question about your documents..."
              value={input}
              onChange={(e) => setInput(e.target.value)}
              disabled={loading}
              sx={{
                "& .MuiOutlinedInput-root": {
                  borderRadius: 3,
                  backgroundColor: "white",
                },
              }}
            />
            <IconButton
              type="submit"
              color="primary"
              disabled={!input.trim() || loading}
              sx={{
                backgroundColor: "primary.main",
                color: "white",
                "&:hover": { backgroundColor: "primary.dark" },
                "&.Mui-disabled": {
                  backgroundColor: "grey.300",
                  color: "grey.500",
                },
                width: 40,
                height: 40,
              }}
            >
              <Send size={18} />
            </IconButton>
          </Box>
        </Paper>
      </Fade>
    </Box>
  );
};
