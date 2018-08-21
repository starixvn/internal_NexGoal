export const StatusOptions = [
    { value: '', label: '-- All --' },
    { value: 'true', label: 'Active' },
    { value: 'false', label: 'Inactive' }
];

export const PerformanceProcess = {
    HumanResource: 1,
    Self: 2,
    ProjectManager: 3,
    Evaluator: 4,
    Reviewer: 5
};

export const Policy = {
    None: 0,
    Read: 1,
    Host: 2
};

export const PaginationOptions = {
    page: 1,
    sizePerPage: 20,
    showTotal: false,
    sizePerPageList: [
        { text: '20', value: 20 },
        { text: '50', value: 50 },
        { text: '100', value: 100 },
        { text: '200', value: 200 }
    ]
};