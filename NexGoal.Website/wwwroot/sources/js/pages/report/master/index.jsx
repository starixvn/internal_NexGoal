import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { Tabs, Tab } from 'react-bootstrap';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import { PerformanceProcess } from '../../../constants';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import ObjectiveMessage from './ObjectiveMessage';
import SearchForm from './SearchForm';
import Table from './Table';
import ViewPADetail from '../../../shared/components/performance-appraisal/ViewPADetail';

export default class MasterReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showModal: false,
            filter: {
                period: null,
                groups: [],
                employees: [],
            },
            tableData: [],
            groupOptions: [],
            employeeOptions: [],
            periodOptions: [],
            objectiveMessageModal: {
                showModal: false,
                message: ''
            },
            viewPADetail: {
                showModal: false,
                paDetail: null,
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees')),
            axios.get(resolveClientUrl('Common/GetPeriodsForReport'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse, periodResponse]) => {
                let groupOptions = groupResponse.data.map((group) => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                this.employees = employeeResponse.data;
                let employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                let periodOptions = periodResponse.data.map(x => ({ value: x.periodId, label: x.name }));

                let filter = {
                    ... this.state.filter,
                    ...{
                        groups: groupOptions,
                        period: periodOptions[0],
                        employees: employeeOptions
                    }
                };

                this.setState({ filter, employeeOptions, groupOptions, periodOptions }, () => this.getRecords());
            }).catch(axiosError);
    }

    getRecords = () => {
        let filter = this.state.filter

        axios.get(resolveClientUrl('MasterReport/GetRecords'), {
            params: {
                periodId: filter.period.value,
                groupIds: filter.groups.map(x => x.value).toString(),
                employeeIds: filter.employees.map(x => x.value).toString(),
            }
        }).then(response => {
            let groups = this.state.groupOptions.filter(g => response.data.some(e => e.groupId === g.value));

            this.setState({
                tableData: groups.map(g => ({
                    group: g.label,
                    records: response.data.filter(e => e.groupId === g.value)
                }))
            });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            let employeeOptions = this.employees.filter(e => value.some(v => v.value === e.groupId))
                .map(e => ({
                    value: e.employeeId,
                    label: `${e.nickName} (${e.fullName})`
                }));

            this.setState({
                employeeOptions: employeeOptions,
                filter: {
                    ...this.state.filter,
                    ...{
                        [name]: value,
                        employees: employeeOptions
                    }
                }
            });
        } else {
            this.setState({
                filter: { ...this.state.filter, ...{ [name]: value } }
            });
        }
    }

    handleOpenObjectiveMessageModal = message => this.setState({
        objectiveMessageModal: {
            message: message,
            showModal: true
        }
    })

    handleViewPADetail = paRecordId => {
        axios(resolveClientUrl(`PA/GetById/${paRecordId}`)).then(response => {
            if (response.data) {
                let pa = response.data;
                let reviewer = pa.processes.filter(x => x.processId === PerformanceProcess.Reviewer)[0].evaluator;

                this.setState({
                    viewPADetail: {
                        showModal: true,
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
                    }
                });
            }
        }).catch(axiosError);
    }

    handleClosePADetail = () => this.setState({ viewPADetail: { showModal: false } })

    handleCloseObjectiveMessageModal = () =>
        this.setState({
            objectiveMessageModal: { showModal: false }
        })

    render() {
        const filter = this.state.filter;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('MasterReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        groupOptions={this.state.groupOptions}
                        periodOptions={this.state.periodOptions}
                        employeeOptions={this.state.employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.getRecords}
                        exportUrl={`MasterReport/Export?${stringify({
                            periodId: filter.period ? filter.period.value : null,
                            groupIds: filter.groups.map(x => x.value).toString(),
                            employeeIds: filter.employees.map(x => x.value).toString(),
                        })}`}
                    />
                    <Table tableData={this.state.tableData} onViewObjectiveMessage={this.handleOpenObjectiveMessageModal} handleViewPADetail={this.handleViewPADetail} />
                </div>
                <ObjectiveMessage
                    message={this.state.objectiveMessageModal.message}
                    showModal={this.state.objectiveMessageModal.showModal}
                    onCloseModal={this.handleCloseObjectiveMessageModal}
                />
                {
                    this.state.viewPADetail.showModal &&
                    <ViewPADetail
                        isShowModal={this.state.viewPADetail.showModal}
                        paRecord={this.state.viewPADetail.paDetail.paRecord}
                        employee={this.state.viewPADetail.paDetail.employee}
                        paProcesses={this.state.viewPADetail.paDetail.paProcesses}
                        onCloseModal={this.handleClosePADetail}
                    />
                }
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<MasterReport />, document.getElementById("master-report-wrapper"));