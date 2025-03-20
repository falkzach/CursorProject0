CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS chat_messages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    stream_id UUID NOT NULL,
    time INTEGER NOT NULL,
    message TEXT NOT NULL,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by UUID,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_by_mod BOOLEAN NOT NULL DEFAULT FALSE,
    edited_by_mod BOOLEAN NOT NULL DEFAULT FALSE,
    edited_by_user BOOLEAN NOT NULL DEFAULT FALSE,
    is_mod_message BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE INDEX IF NOT EXISTS idx_chat_messages_user_id ON chat_messages(user_id);
CREATE INDEX IF NOT EXISTS idx_chat_messages_stream_id ON chat_messages(stream_id); 