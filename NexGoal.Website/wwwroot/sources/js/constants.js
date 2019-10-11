export const DateFormat = 'DD MMM YYYY';
export const EmployeeRoleId = 101;
export const notAvailableGradeTooltip = 'This member will have the lowest grade (Unsatisfactory)';

export const NotificationStatus = {
    Unread: 0,
    Read: 1
};

export const StatusOptions = [
    { value: '', label: '-- All --' },
    { value: 'true', label: 'Active' },
    { value: 'false', label: 'Inactive' }
];

export const PAEditOptions = [
    { value: '', label: '-- All --' },
    { value: true, label: 'Yes' },
    { value: false, label: 'No' }
];

export const TargetType = {
    Probation: 1,
    SemiAnnual: 2
};

export const PerformanceProcess = {
    HumanResource: 1,
    Self: 2,
    ProjectManager: 3,
    Evaluator: 4,
    Reviewer: 5
};

export const PAStatus = {
    NotGenerated: 1,
    Opened: 2,
    InProgress: 3,
    Closed: 4,
    Rejected: 5
};

export const Policy = {
    None: 0,
    Read: 1,
    Host: 2
};

export const PerformanceRecordStatus = {
    New: 1,
    Draft: 2,
    Submitted: 3,
    Closed: 4
};

export const PerformanceRecordType = {
    PerformanceCheckin: 1,
    PerformanceAppraisal: 2,
    Nomination: 3,
    SystemPerformanceAppraisal: 4,
    SystemNomination: 5
};

export const NominationStatus = {
    Nominated: 1,
    Shortlisted: 2,
    Winner: 3
};

export const NominationStatuses = [
    { value: 1, text: 'Nominated' },
    { value: 2, text: 'Shortlisted' },
    { value: 3, text: 'Winner' }
];

export const ProbationTag = {
    value: -1,
    label: 'Probation'
};

export const NotificationActionType = {
    MyPageWithCreatePerformanceCheckin: 1,
    MyPageWithSpecificPerformanceCheckin: 2,
    MemberPageWithCreatePerformanceCheckin: 3,
    MemberPageWithSpecificPerformanceCheckin: 4,
    MemberPageWithActiveNominationTab: 5,
    MemberPageWithActivePerformanceAppraisalTab: 6,
    PAFocus: 7,
    ProbationSettingPage: 8,
    MemberPageWithActiveProbationTab: 9
};

export const PaginationOptions = {
    sizePerPage: 20,
    showTotal: false,
    sizePerPageList: [
        { text: '20', value: 20 },
        { text: '50', value: 50 },
        { text: '100', value: 100 },
        { text: '200', value: 200 }
    ]
};

export const ChartColors = ['#1B1464', '#F3AC16', '#838BC5', '#BA9BC9', '#39B54A', '#0071BC'];

export const InputErrorCssClass = 'input-validation-error';

export const TextCenterCssClass = 'text-center';