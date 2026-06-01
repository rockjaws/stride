-- ===========================================
--         Stride - Seed Data
-- ===========================================

-- Users
INSERT INTO Users (FirstName, LastName, WorkMail) VALUES
('Alice', 'Hansen', 'alice.hansen@stride.dev'),
('Bob', 'Nielsen', 'bob.nielsen@stride.dev'),
('Clara', 'Jensen', 'clara.jensen@stride.dev');

-- Projects
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
);

-- Assign all users to both projects
INSERT INTO ProjectUser (ProjectsId, UsersId) VALUES
(1, 1), (1, 2), (1, 3),
(2, 1), (2, 2);

-- Chat channels
INSERT INTO ChatChannels (Name, ProjectId) VALUES
('general', 1),
('backend', 1),
('general', 2);

-- Tasks for Project 1
INSERT INTO ProjectTasks (
    Title, Description, StartDate, Deadline, Progress, Priority, ProjectId
) VALUES
(
    'Set up SQLite database',
    'Configure EF Core with SQLite and run initial migrations.',
    '2026-05-01',
    '2026-05-05',
    'Done',
    'High',
    1
),
(
    'Build Kanban board UI',
    'Implement the four-column kanban view in WPF using MVVM.',
    '2026-05-06',
    '2026-05-20',
    'InProgress',
    'High',
    1
),
(
    'Implement drag-and-drop',
    'Allow tasks to be dragged between Kanban columns.',
    '2026-05-21',
    '2026-06-05',
    'Backlog',
    'Normal',
    1
),
(
    'Write unit tests',
    'Cover ViewModels and services with xUnit tests.',
    '2026-06-01',
    '2026-06-30',
    'Backlog',
    'Low',
    1
),
(
    'Add notification support',
    'Wire up the notifications endpoint to the WPF client.',
    '2026-05-10',
    '2026-05-25',
    'Review',
    'Normal',
    1
);

-- Tasks for Project 2
INSERT INTO ProjectTasks (
    Title, Description, StartDate, Deadline, Progress, Priority, ProjectId
) VALUES
(
    'Design app wireframes',
    'Create Figma wireframes for the mobile companion app.',
    '2026-06-01',
    '2026-06-15',
    'Done',
    'Normal',
    2
),
(
    'Set up MAUI project',
    'Scaffold .NET MAUI solution and configure API client.',
    '2026-06-10',
    '2026-06-20',
    'InProgress',
    'High',
    2
);

-- Assign users to tasks
INSERT INTO ProjectTaskUser (ProjectTasksId, UsersId) VALUES
(1, 1),       -- Alice on "Set up SQLite"
(2, 1),       -- Alice on "Build Kanban"
(2, 2),       -- Bob   on "Build Kanban"
(3, 2),       -- Bob   on "Drag-and-drop"
(4, 3),       -- Clara on "Unit tests"
(5, 3),       -- Clara on "Notifications"
(6, 1),       -- Alice on "Wireframes"
(7, 2);       -- Bob   on "MAUI project"
