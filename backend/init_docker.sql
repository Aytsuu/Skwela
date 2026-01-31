CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Users" (
    user_id uuid NOT NULL,
    username text NOT NULL,
    password text NOT NULL,
    email text NOT NULL,
    display_name character varying(200) NOT NULL,
    display_image text NOT NULL,
    role integer NOT NULL,
    user_created_at timestamp with time zone NOT NULL,
    "refreshToken" text,
    "refreshTokenExpiryTime" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY (user_id)
);

CREATE TABLE "Classrooms" (
    class_id uuid NOT NULL,
    class_name character varying(200) NOT NULL,
    class_description character varying(1000) NOT NULL,
    class_created_at timestamp with time zone NOT NULL,
    user_id uuid NOT NULL,
    CONSTRAINT "PK_Classrooms" PRIMARY KEY (class_id),
    CONSTRAINT "FK_Classrooms_Users_user_id" FOREIGN KEY (user_id) REFERENCES "Users" (user_id) ON DELETE CASCADE
);

CREATE TABLE "Enrollments" (
    user_id uuid NOT NULL,
    class_id uuid NOT NULL,
    enrolled_at timestamp with time zone NOT NULL,
    enrolled_status character varying(50) NOT NULL,
    CONSTRAINT "PK_Enrollments" PRIMARY KEY (user_id),
    CONSTRAINT "FK_Enrollments_Classrooms_class_id" FOREIGN KEY (class_id) REFERENCES "Classrooms" (class_id) ON DELETE CASCADE,
    CONSTRAINT "FK_Enrollments_Users_user_id" FOREIGN KEY (user_id) REFERENCES "Users" (user_id) ON DELETE CASCADE
);

CREATE INDEX "IX_Classrooms_class_id" ON "Classrooms" (class_id);

CREATE INDEX "IX_Classrooms_user_id" ON "Classrooms" (user_id);

CREATE INDEX "IX_Enrollments_class_id" ON "Enrollments" (class_id);

CREATE INDEX "IX_Users_role" ON "Users" (role);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260131184643_InitialCreate', '8.0.0');

COMMIT;

