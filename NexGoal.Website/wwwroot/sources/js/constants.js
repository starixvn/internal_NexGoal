export const DateFormat = 'DD MMM, YYYY';

export const StatusOptions = [
    { value: '', label: '-- All --' },
    { value: 'true', label: 'Active' },
    { value: 'false', label: 'Inactive' }
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

export const PerformanceRecordSizePerPages = [
    { text: '5', value: 5 },
    { text: '10', value: 10 },
    { text: '20', value: 20 },
    { text: '50', value: 50 }
];