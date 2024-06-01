DROP TABLE IF EXISTS certificate;
DROP TABLE IF EXISTS certificate_file;
DROP TABLE IF EXISTS event_proccess;
DROP TABLE IF EXISTS event_proccess_status;

CREATE TABLE certificate (
  id SERIAL PRIMARY KEY,
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

CREATE TABLE certificate_file (
  id SERIAL PRIMARY KEY,
  id_certificate INTEGER,
  png VARCHAR(255) NOT NULL,
  png_path VARCHAR(255) NOT NULL,
  png_size INTEGER NOT NULL,
  pdf VARCHAR(255) NOT NULL,
  pdf_path VARCHAR(255) NOT NULL,
  pdf_size INTEGER NOT NULL,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW(),
  CONSTRAINT fk_certificate FOREIGN KEY (id_certificate) REFERENCES certificate (id) ON DELETE CASCADE
);

CREATE TABLE event_proccess_status (
  id SERIAL PRIMARY KEY,
  status VARCHAR(50) NOT NULL,
  active BOOLEAN NOT NULL DEFAULT false,
  created TIMESTAMP NOT NULL DEFAULT NOW(),
  updated TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE event_proccess (
  id SERIAL PRIMARY KEY,
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