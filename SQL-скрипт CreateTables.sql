-- Таблица паспортов
CREATE TABLE passports (
    id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    number VARCHAR(50) NOT NULL UNIQUE
);

-- Таблица отделов
CREATE TABLE departments (
    id SERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL
);

-- Таблица сотрудников
CREATE TABLE employees (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    company_id INTEGER NOT NULL,
    department_id INTEGER NOT NULL,
    passport_id INTEGER NOT NULL,
    CONSTRAINT fk_department FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE RESTRICT,
    CONSTRAINT fk_passport FOREIGN KEY (passport_id) REFERENCES passports(id) ON DELETE CASCADE,
    CONSTRAINT uk_passport UNIQUE (passport_id)
);

-- Создаем индексы для производительности
CREATE INDEX idx_employees_company_id ON employees(company_id);
CREATE INDEX idx_employees_department_id ON employees(department_id);
CREATE INDEX idx_departments_company_id ON departments(company_id);
