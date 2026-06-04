CREATE TABLE users (
                       id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                       identifier    VARCHAR UNIQUE NOT NULL, -- matrícula hoje, CPF amanhã
                       password_hash VARCHAR NOT NULL,
                       roles         VARCHAR NOT NULL,
                       created_at    TIMESTAMP DEFAULT NOW()
);

CREATE TABLE companies (
                           id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                           name       VARCHAR NOT NULL,
                           is_active  BOOLEAN DEFAULT TRUE,
                           user_id    UUID NOT NULL REFERENCES users(id),
                           created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE employees (
                           id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                           name       VARCHAR NOT NULL,
                           job        VARCHAR,
                           user_id    UUID NOT NULL REFERENCES users(id),
                           company_id UUID NOT NULL REFERENCES companies(id),
                           created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE time_records (
                              id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                              employee_id UUID NOT NULL REFERENCES employees(id),
                              company_id  UUID NOT NULL REFERENCES companies(id),
                              type        VARCHAR NOT NULL,
                              recorded_at TIMESTAMP DEFAULT NOW()
);