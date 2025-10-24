CREATE TABLE IF NOT EXISTS tags
(
    id   BIGSERIAL PRIMARY KEY,
    name text UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS todo_tags
(
    todo_id bigint NOT NULL REFERENCES todos (id) ON DELETE CASCADE,
    tag_id  bigint NOT NULL REFERENCES tags (id) ON DELETE CASCADE,
    PRIMARY KEY (todo_id, tag_id)
);