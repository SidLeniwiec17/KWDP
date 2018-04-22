CREATE TABLE ecg(
	id INTEGER PRIMARY KEY,
	date TEXT,
	ecg TEXT
);

CREATE TABLE patient(
	id INTEGER PRIMARY KEY,
	name TEXT,
	surname TEXT,
	age INTEGER,
	sex INTEGER,
	pesel TEXT,
	height INTEGER,
	weight INTEGER,
	ecg_id INTEGER,
	FOREIGN KEY(ecg_id) REFERENCES ecg(id)
);

CREATE TABLE question(
	id INTEGER PRIMARY KEY,
	content TEXT
);

CREATE TABLE patient_answer(
	patient_id INTEGER,
	question_id INTEGER,
	answer TEXT,
	PRIMARY KEY(patient_id, question_id),
	FOREIGN KEY(patient_id) REFERENCES patient(id),
	FOREIGN KEY(question_id) REFERENCES question(id)
);