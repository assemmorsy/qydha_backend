CREATE TABLE update_email_requests (
    id uuid DEFAULT uuid_generate_v4() PRIMARY KEY NOT NULL,
    email VARCHAR(100)  NOT NULL,
    otp VARCHAR(6) NOT NULL, 
    created_on TIMESTAMP NOT NULL,
    user_id uuid NOT NULL
);