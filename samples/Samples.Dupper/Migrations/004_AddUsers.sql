CREATE TABLE IF NOT EXISTS users
(
    id               uuid PRIMARY KEY,
    email            text        NOT NULL,
    normalized_email text UNIQUE NOT NULL,
    created_at       timestamptz NOT NULL
);

DELETE FROM todos
    WHERE true = true;

ALTER TABLE todos
    ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL REFERENCES users(id);