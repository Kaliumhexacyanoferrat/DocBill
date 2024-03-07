-- ISSUER

CREATE TABLE issuer
(
    id           SERIAL       NOT NULL,
    name         VARCHAR(255) NOT NULL,
    created      TIMESTAMP    NOT NULL,
    modified     TIMESTAMP    NOT NULL,
    PRIMARY KEY (id)
);

CREATE UNIQUE INDEX ux_issuer_name
    ON issuer (name);

-- BILL

CREATE TABLE bill
(
    id           SERIAL       NOT NULL,
    status       SMALLINT     NOT NULL,
    issuer       INT          NOT NULL,
    number       VARCHAR(255) NOT NULL,
    created      TIMESTAMP    NOT NULL,
    modified     TIMESTAMP    NOT NULL,
    PRIMARY KEY (id)
);

CREATE UNIQUE INDEX ix_bill_number
    ON bill (number);
    
CREATE INDEX ix_bill_status
    ON bill (status);

CREATE INDEX ix_bill_created
    ON bill (created DESC);

CREATE INDEX ix_bill_modified
    ON bill (modified DESC);
    
ALTER TABLE bill
    ADD CONSTRAINT fk_bill_issuer FOREIGN KEY (issuer) REFERENCES issuer (id) ON DELETE CASCADE;
