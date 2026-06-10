-- ============================================================================
--                               Stride - Comprehensive Seed Data
-- ============================================================================

-- 1. Users
INSERT INTO Users (FirstName, LastName, WorkMail) VALUES
('Alice', 'Hansen', 'alice.hansen@stride.dev'),
('Bob', 'Nielsen', 'bob.nielsen@stride.dev'),
('Clara', 'Jensen', 'clara.jensen@stride.dev'),
('Oliver', 'Andersen', 'oliver.andersen@stride.dev'),

-- User tests data --

('Martin', 'Efternavn1', '@dania.dk'),
('Camilla', 'Efternavn2', '@dania.dk'),
('Casper', 'Efternavn3', '@dania.dk'),
('Daniel', 'Efternavn4', '@dania.dk');

-- 2. Projects (Showcasing Active vs. Archived States)
INSERT INTO Projects (
    Title, Description, StartDate, Deadline, IsArchived
) VALUES
(
    'Stride Platform',
    'Internal project management tool built with WPF and ASP.NET Core.',
    '2026-05-01',
    '2026-08-31',
    0
),
(
    'Mobile Companion App',
    'Cross-platform mobile client for Stride using .NET MAUI.',
    '2026-06-01',
    '2026-12-01',
    0
),
(
    'Legacy POS System',
    'Old restaurant point-of-sale system framework. Retained for architectural history.',
    '2025-01-15',
    '2025-06-01',
    1
);

-- 3. Project Membership (Many-to-Many: ProjectUser)
INSERT INTO ProjectUser (ProjectsId, UsersId) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), -- All hands on main Stride Platform
(2, 1), (2, 2),                 -- Alice & Bob exploring MAUI app
(3, 3), (3, 4);                 -- Clara & Oliver handled the legacy system

-- 4. Chat Channels (Scoping to distinct project contexts)
INSERT INTO ChatChannels (Name, ProjectId) VALUES
('general', 1),
('backend', 1),
('frontend-wpf', 1),
('general', 2),
('ui-design', 2),
('archive-records', 3);

-- 5. Project Tasks (Exhaustive verification of TaskProgress and TaskPriority Enums)
INSERT INTO ProjectTasks (
    Title, Description, StartDate, Deadline, Progress, Priority, ProjectId
) VALUES
-- Project 1 Tasks
(
    'Set up SQLite database',
    'Configure EF Core with SQLite and execute initial migrations.',
    '2026-05-01',
    '2026-05-05',
    'Done',
    'High',
    1
),
(
    'Build Kanban board UI',
    'Implement the four-column kanban view grid layout in WPF using MVVM.',
    '2026-05-06',
    '2026-05-20',
    'InProgress',
    'High',
    1
),
(
    'Implement drag-and-drop',
    'Allow tasks to be smoothly dragged between kanban state swimlanes.',
    '2026-05-21',
    '2026-06-05',
    'Backlog',
    'Normal',
    1
),
(
    'Write unit tests',
    'Attain code test coverage on ViewModels and core application services using xUnit.',
    '2026-06-01',
    '2026-06-30',
    'Backlog',
    'Low',
    1
),
(
    'Add notification support',
    'Wire up the notifications API hub endpoint directly to the desktop WPF client UI alert system.',
    '2026-05-10',
    '2026-05-25',
    'Review',
    'Normal',
    1
),

-- Project 2 Tasks
(
    'Design app wireframes',
    'Create clean high-fidelity Figma components and layouts for the mobile client.',
    '2026-06-01',
    '2026-06-15',
    'Done',
    'Normal',
    2
),
(
    'Set up MAUI project',
    'Scaffold cross-platform .NET MAUI base framework solution and configure common HTTP client.',
    '2026-06-10',
    '2026-06-20',
    'InProgress',
    'High',
    2
),

-- Project 3 Tasks (Legacy data mapping back to archived context)
(
    'Finalize database backup',
    'Export baseline data sets before freezing repository.',
    '2025-05-01',
    '2025-05-15',
    'Done',
    'Low',
    3
);

-- 6. Task Assignment Matrix (Many-to-Many: ProjectTaskUser)
INSERT INTO ProjectTaskUser (ProjectTasksId, UsersId) VALUES
(1, 1),          -- Alice on "Set up SQLite"
(2, 1), (2, 2),  -- Alice & Bob collaborating on "Build Kanban Board"
(3, 2),          -- Bob on "Drag-and-drop"
(4, 3),          -- Clara on "Unit tests"
(5, 3), (5, 4),  -- Clara & Oliver pushing "Notification Support" through review
(6, 1),          -- Alice on "Wireframes"
(7, 2),          -- Bob on "MAUI project"
(8, 4);          -- Oliver on old archived task

-- 7. Chat Channel Messaging logs
INSERT INTO Messages (Text, Time, ChannelId, UserId) VALUES
-- Alice in backend
(
    'Hey team, I ran the database migrations smoothly. Let me know if you run into any SQLite constraints locally.',
    '2026-05-04 10:15:00',
    2,
    1
),
-- Bob replying in backend
(
    'Awesome work Alice, pulling down the updates now to start building out the WPF layouts.',
    '2026-05-04 10:32:00',
    2,
    2
),
-- Alice in ui-design
(
    'Does anyone have preferences on the canvas dimensions for our Figma mobile layouts?',
    '2026-06-02 09:00:00',
    5,
    1
),
-- Bob in ui-design
(
    'Stick to standard aspect ratios first, we can test responsiveness inside the MAUI views later.',
    '2026-06-02 09:45:00',
    5,
    2
);

-- 8. Notifications (Demonstrating Read/Unread flag logic and optional Nullable Foreign Keys)
INSERT INTO Notifications (Text, IsRead, Time, UserId, TaskId, ProjectId) VALUES
-- Task-specific notifications
(
    'You have been assigned to: Build Kanban board UI',
    1,
    '2026-05-06 08:00:00',
    2,
    2,
    1
),
(
    'Task "Add notification support" was moved to Review column',
    0,
    '2026-06-04 16:30:00',
    4,
    5,
    1
),
(
    'Task "Set up SQLite database" was completed by Alice',
    1,
    '2026-05-05 17:00:00',
    3,
    1,
    1
),

-- General Project-wide notifications (TaskId left NULL as allowed by schema)
(
    'Project "Mobile Companion App" has been successfully initiated.',
    0,
    '2026-06-01 08:30:00',
    1,
    NULL,
    2
),
(
    'Project "Mobile Companion App" has been successfully initiated.',
    1,
    '2026-06-01 08:30:00',
    2,
    NULL,
    2
);
