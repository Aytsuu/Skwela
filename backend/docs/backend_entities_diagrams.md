# Backend Entities Diagrams

Since entities are typically structural, an **Entity-Relationship (ER) diagram** has been generated to show the data schema and relationships. Additionally, a **Sequence diagram** is included to illustrate the lifecycle and typical interactions among these entities.

## Entity-Relationship (ER) Diagram

```mermaid
erDiagram
    User ||--o{ Classroom : creates
    User ||--o{ Notification : receives
    Classroom ||--o{ Assessment : contains
    Assessment ||--o{ Question : has
    Assessment ||--o{ Record : has
    Record ||--o{ RecordAnswer : contains
    Question ||--o{ RecordAnswer : corresponds_to

    User {
        Guid user_id PK
        string email
        string password
        string display_name
        string display_image
        bool is_admin
        bool is_email_verified
        DateTime user_created_at
        DateTime user_updated_at
        string refreshToken
        DateTime refreshTokenExpiryTime
    }

    Classroom {
        Guid class_id PK
        string class_name
        string class_description
        string class_banner
        bool class_is_archived
        DateTime class_created_at
        DateTime class_updated_at
        Guid user_id FK
    }

    Assessment {
        Guid ass_id PK
        string ass_title
        string ass_type
        string ass_answer_key_url
        string ass_instruction
        float ass_total_points
        string ass_status
        DateTime ass_created_at
        DateTime ass_updated_at
        Guid class_id FK
    }

    Question {
        Guid quest_id PK
        int quest_num
        string quest_type
        string quest_text
        string quest_correct_answer
        string quest_rubric
        float quest_max_points
        float quest_ai_confidence
        Guid ass_id FK
    }

    Record {
        Guid rec_id PK
        string rec_student_name
        string rec_scan_url
        float rec_total_score
        float rec_percentage
        string rec_status
        DateTime rec_graded_at
        DateTime rec_created_at
        Guid ass_id FK
    }

    RecordAnswer {
        Guid ra_id PK
        string ra_student_ans
        float ra_awarded_pts
        float ra_ai_confidence
        string ra_feedback
        bool ra_teacher_rev
        float ra_teacher_op
        string ra_raw_response
        Guid quest_id FK
        Guid rec_id FK
    }
    
    Notification {
        Guid notif_id PK
        Guid user_id FK
        string notif_title
        string notif_message
        bool notif_is_read
        DateTime notif_created_at
    }
```

## Entity Lifecycle Sequence Diagram

This sequence diagram demonstrates the logical flow of how these entities are created and interact with each other throughout a typical assessment process.

```mermaid
sequenceDiagram
    actor Teacher
    participant User as User Entity
    participant Classroom as Classroom Entity
    participant Assessment as Assessment Entity
    participant Question as Question Entity
    actor Student
    participant Record as Record Entity
    participant RecordAnswer as RecordAnswer Entity
    participant Notification as Notification Entity

    Teacher->>User: Register/Login
    Teacher->>Classroom: Build(userId, name, description, banner)
    Teacher->>Assessment: Build(title, type, instruction, classId)
    Teacher->>Question: Build(num, type, text, answer, rubric, assId)
    
    Note over Student, Record: Student submits their answers
    Student->>Record: Build(studentName, scanUrl, assId)
    Record->>RecordAnswer: Build(studentAnswer, awardedPts, questId, recId)
    
    Note over RecordAnswer, Question: System AI matches and grades
    RecordAnswer-->>Question: Validate against quest_correct_answer / quest_rubric
    RecordAnswer-->>Record: Calculate rec_total_score and rec_percentage
    
    Record-->>Assessment: Link Record to Assessment
    Record->>Notification: Build(userId, title, message)
    Notification-->>Teacher: Deliver "Assessment Submitted" Notification
```
