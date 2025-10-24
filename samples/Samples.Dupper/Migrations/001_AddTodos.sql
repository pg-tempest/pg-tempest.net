CREATE TABLE IF NOT EXISTS todos
(
    id          BIGSERIAL PRIMARY KEY,
    title       text        NOT NULL,
    description text,
    completed   bool        NOT NULL,
    created_at  timestamptz NOT NULL
);
