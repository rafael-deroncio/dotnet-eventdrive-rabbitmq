-- Drop existing tables if they exist
DROP TABLE IF EXISTS certificate_files;
DROP TABLE IF EXISTS file_details;
DROP TABLE IF EXISTS event_proccess;
DROP TABLE IF EXISTS event_proccess_status;
DROP TABLE IF EXISTS certificates;

-- Drop existing sequences if they exist
DROP SEQUENCE IF EXISTS certificates_id_seq;
DROP SEQUENCE IF EXISTS file_details_id_seq;
DROP SEQUENCE IF EXISTS certificate_files_id_seq;
DROP SEQUENCE IF EXISTS event_proccess_id_seq;

-- Create sequences
CREATE SEQUENCE certificates_id_seq;
CREATE SEQUENCE file_details_id_seq;
CREATE SEQUENCE certificate_files_id_seq;
CREATE SEQUENCE event_proccess_id_seq;

-- Set the starting value of the sequences to a random number
SELECT setval('certificates_id_seq', round(random() * (99999 - 10000) + 10000)::bigint);
SELECT setval('file_details_id_seq', round(random() * (99999 - 10000) + 10000)::bigint);
SELECT setval('certificate_files_id_seq', round(random() * (99999 - 10000) + 10000)::bigint);
SELECT setval('event_proccess_id_seq', round(random() * (99999 - 10000) + 10000)::bigint);


-- Create the certificates table with random sequence
CREATE TABLE certificates (
  id INTEGER PRIMARY KEY DEFAULT nextval('certificates_id_seq'),
  student VARCHAR(100) NOT NULL,
  document VARCHAR(100) NOT NULL,
  registration VARCHAR(50) NOT NULL,
  course VARCHAR(100) NOT NULL,
  completion DATE NOT NULL,
  utilization NUMERIC(5, 2) NOT NULL,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create the file_details table with random sequence
CREATE TABLE file_details (
  id INTEGER PRIMARY KEY DEFAULT nextval('file_details_id_seq'),
  file TEXT NOT NULL,
  type VARCHAR(50) NOT NULL,
  path VARCHAR(255) NOT NULL,
  size INTEGER NOT NULL,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create the certificate_files table with random sequence
CREATE TABLE certificate_files (
  id INTEGER PRIMARY KEY DEFAULT nextval('certificate_files_id_seq'),
  id_certificate INTEGER,
  id_file INTEGER NOT NULL,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW(),
  CONSTRAINT fk_certificate FOREIGN KEY (id_certificate) REFERENCES certificates (id) ON DELETE CASCADE,
  CONSTRAINT fk_file FOREIGN KEY (id_file) REFERENCES file_details (id) ON DELETE CASCADE
);

-- Create the event_proccess_status table with random sequence
CREATE TABLE event_proccess_status (
  id INTEGER PRIMARY KEY,
  status VARCHAR(50) NOT NULL,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW(),
  CONSTRAINT ck_status CHECK (status IN ('OnProcecess', 'Pending', 'Success', 'Error'))
);

-- Insert initial values into event_proccess_status with random IDs
INSERT INTO event_proccess_status (id, status)
VALUES 
    (1, 'OnProcecess'),
    (2, 'Pending'),
    (3, 'Success'),
    (4, 'Error');

-- Create the event_proccess table with random sequence
CREATE TABLE event_proccess (
  id INTEGER PRIMARY KEY DEFAULT nextval('event_proccess_id_seq'),
  id_status INTEGER NOT NULL,
  error TEXT NOT NULL,
  attempts INTEGER NOT NULL DEFAULT 0,
  json_event JSONB NOT NULL,
  finished BOOLEAN NOT NULL DEFAULT false,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW(),
  CONSTRAINT fk_event_status FOREIGN KEY (id_status) REFERENCES event_proccess_status (id) ON DELETE CASCADE
);
