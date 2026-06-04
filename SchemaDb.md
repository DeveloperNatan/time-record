users
├── id
├── matriculation    (matrícula ou CPF — único)
├── password_hash
├── roles         ('company_admin', 'employee')
└── created_at

companies
├── id
├── name
├── is_active
├── user_id       → users.id
└── created_at

employees
├── id
├── name
├── job
├── user_id       → users.id
├── company_id    → companies.id
└── created_at

time_records
├── id
├── employee_id   → employees.id
├── company_id    → companies.id
├── type          ('clock_in', 'clock_out')
└── recorded_at