CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Users (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    username VARCHAR(100) UNIQUE,
    name VARCHAR(200),
    password_hash VARCHAR,
    phone VARCHAR(30) UNIQUE,
    email VARCHAR(200) UNIQUE,
    is_anonymous BOOLEAN DEFAULT(false) not null,
    birth_date Date,
    created_on TIMESTAMP NOT NULL,
    last_login TIMESTAMP,
    is_phone_confirmed BOOLEAN DEFAULT(false) not null,
    is_email_confirmed BOOLEAN DEFAULT(false) not null,
    avatar_url VARCHAR,
    avatar_path VARCHAR
);