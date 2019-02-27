import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import { PerformanceProcess } from '../../../constants';
import Filter from './filter';
import Table from './table';
import ViewPADetail from '../../../shared/components/performance-appraisal/ViewPADetail';

export default class App extends Component {
    constructor(props) {
        super(props);
        this.state = {
            filter: {
                periods: [],
                groups: [],
                employees: []
            },
            paPeriodRecords: null,
            isShowPADetail: false,
            paDetailId: null,
            employeeOptions: []
        };

        this.periodOptions = [];
        this.groupOptions = [];
        this.employees = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPeriodsForReport')),
            axios(resolveClientUrl('Report/GetGroupAndMember'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupAndMemberResponse]) => {
                this.periodOptions = periodResponse.data.map((period) => {
                    return {
                        label: period.name,
                        value: period.periodId
                    };
                });

                this.groupOptions = groupAndMemberResponse.data.groups.map((group) => {
                    return {
                        label: group.name,
                        value: group.groupId
                    };
                });
                this.employees = groupAndMemberResponse.data.members.map((employee) => {
                    return {
                        label: `${employee.nickName} (${employee.fullName})`,
                        value: employee.employeeId,
                        groupId: employee.groupId
                    };
                });
                this.setState({
                    filter: {
                        periods: this.periodOptions[0] ? [this.periodOptions[0]] : [],
                        groups: this.groupOptions,
                        employees: this.employees
                    },
                    employeeOptions: this.employees
                }, () => this.getPARecords());
            }).catch(axiosError);
    }

    handleFilterChange = (filterName, values) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [filterName]: values })
        }, () => {
            if (filterName === 'groups') {
                const groupIds = values.length > 0 ? values.map(group => group.value) : [];
                this.onGroupChange(groupIds);
            }
        });
    }

    onGroupChange = (selectedGroups) => {
        let employees = [...this.employees];
        let employeeOptions = employees.filter(employee => selectedGroups.includes(employee.groupId))
        let filter = this.state.filter;
        filter = Object.assign({}, this.state.filter, { employees: employeeOptions })
        this.setState({
            filter: filter,
            employeeOptions: employeeOptions
        });
    }

    handleClickViewPADetail = (paRecordId) => {
        axios(resolveClientUrl(`PA/GetById/${paRecordId}`)).then(response => {
            if (response.data) {
                let pa = response.data;
                let reviewer = pa.processes.filter(x => x.processId === PerformanceProcess.Reviewer)[0].evaluator;

                this.setState({
                    isShowPADetail: true,
                    paDetail: {
                        paRecord: {
                            paRecordId: pa.paRecordId,
                            grade: pa.grade,
                            comment: pa.comment,
                            reviewer: reviewer,
                            modifiedDate: pa.modifiedDate
                        },
                        employee: pa.employee,
                        paProcesses: pa.processes
                    }
                });
            }
        }).catch(axiosError);
    }

    handleClosePADetailModel = () => {
        this.setState({
            isShowPADetail: false
        });
    }

    handleFilterSubmit = () => {
        this.getPARecords();
    }

    handleExportExcel = () => {
        const filter = this.state.filter;

        axios({
            url: resolveClientUrl('report/ExportPAReport'),
            params: {
                periodIds: filter.periods.map((period) => { return period.value; }).join(','),
                groupIds: filter.groups.map(group => { return group.value; }).join(','),
                employeeIds: filter.employees.map(employee => { return employee.value; }).join(',')
            },
            method: 'get',
            responseType: 'blob'
        }).then((response) => {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            let link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `PA Report.xlsx`);
            document.body.appendChild(link);
            link.click();
        }).catch(axiosError);
    }

    getPARecords = () => {
        const filter = this.state.filter;
        axios.get(resolveClientUrl('Report/GetPARecords'), {
            params: {
                periodIds: filter.periods.map((period) => { return period.value; }).join(','),
                groupIds: filter.groups.map(group => { return group.value; }).join(','),
                employeeIds: filter.employees.map(employee => { return employee.value; }).join(',')
            }
        }).then(response => {
            this.setState({
                paPeriodRecords: response.data
            });
        }).catch(axiosError);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PerformanceAppraisalReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Filter periodOptions={this.periodOptions}
                        groupOptions={this.groupOptions}
                        employeeOptions={this.state.employeeOptions}
                        displayMemberFilter={this.employees.length > 1}
                        filter={this.state.filter}
                        onFilterChange={this.handleFilterChange}
                        onSubmit={this.handleFilterSubmit}
                        exportExcel={this.handleExportExcel}
                    />
                    <Table periodRecords={this.state.paPeriodRecords}
                        showPaDetail={this.handleClickViewPADetail}
                    />
                    {
                        this.state.isShowPADetail &&
                        <ViewPADetail
                            isShowModal={this.state.isShowPADetail}
                            paRecord={this.state.paDetail.paRecord}
                            employee={this.state.paDetail.employee}
                            paProcesses={this.state.paDetail.paProcesses}
                            onCloseModal={this.handleClosePADetailModel}
                        />
                    }
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('pa-report'));