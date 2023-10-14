CREATE TABLE Purchases (
    id uuid DEFAULT uuid_generate_v4(),
    iaphub_purchase_id VARCHAR(40),
    user_id uuid NOT NULL,
    type VARCHAR(10) NOT NULL,
    purchase_date TIMESTAMP NOT NULL ,
    productSku VARCHAR(15) NOT NULL,
    number_of_days INT NOT NULL,
    PRIMARY KEY(id),
    CONSTRAINT fk_user
        FOREIGN KEY(user_id)
            REFERENCES Users(id)
            ON DELETE CASCADE
);