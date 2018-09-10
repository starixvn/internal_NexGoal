import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import Async from 'react-select/lib/Async';

import { axiosError, noDataIndication, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/multi-select/index';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PaginationOptions } from '../../../constants';
import { PAStatus, PerformanceProcess } from '../../../constants';
import Edit from './Edit';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            addFilter: {
                groups: [],
                employee: null
            },
            showEditModal: false,
            showConfirmationModal: false,
            selectedSettingToUpdateStatus: null
        };
    }

    selectedIds = []

    columns = [
        {
            dataField: 'rowNum',
            text: `${getResource('SymbolCount')}`,
            classes: 'text-center',
            headerStyle: { width: '50px' }
        },
        {
            dataField: 'group',
            classes: 'text-center',
            text: 'Team'
        },
        {
            dataField: 'fullName',
            text: '',
            hidden: true
        },
        {
            dataField: 'nickName',
            text: 'Employee',
            formatter: (cell, row, rowIndex, colIndex) => {
                return `${row.fullName} (${row.nickName})`;
            }
        },
        {
            dataField: 'evaluator',
            text: 'Evaluator',
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                var evaluatorProcess = row.processes.find(p => p.processId === PerformanceProcess.Evaluator);

                return (
                    <Fragment>
                        <div>{evaluatorProcess.evaluator}</div>
                        <div>{evaluatorProcess.status}</div>
                    </Fragment>
                );
            }
        },
        {
            dataField: 'reviewer',
            text: 'Reviewer',
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                var reviewerProcess = row.processes.find(p => p.processId === PerformanceProcess.Reviewer);

                return (
                    <Fragment>
                        <div>{reviewerProcess.evaluator}</div>
                        <div>
                            {reviewerProcess.startDate && moment(reviewerProcess.startDate).format('DD MMM, YYYY')}
                            {reviewerProcess.endDate && ` - ${moment(reviewerProcess.endDate).format('DD MMM, YYYY')}`}
                        </div>
                        <div>{reviewerProcess.status}</div>
                    </Fragment>
                );
            }
        },
        {
            dataField: 'status',
            text: 'PA Status',
            classes: 'text-center'
        },
        {
            dataField: 'statusId',
            text: 'Include',
            classes: 'text-center',
            headerStyle: { width: '80px' },
            formatter: (cell, row, rowIndex, colIndex) => {
                return (
                    <input type="checkbox" checked={row.statusId !== PAStatus.Rejected} onChange={() => this.editSettingStatusConfirmation(row)} />
                );
            }
        },
        {
            dataField: '',
            text: 'Action',
            classes: 'text-center',
            headerStyle: { width: '80px' },
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                return (
                    <a className="fa fa-pencil-square-o" title="Edit" onClick={() => this.editSetting(row.paRecordId)} />
                );
            }
        }]

    searchEmployees = (input, callback) => {
        if (input.length > 2) {
            return axios(`/common/SearchEmployees?keyword=${input}`)
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: `${item.fullName} (${item.nickName})`
                        });
                    });

                    return callback(options);
                });
        }

        return callback([]);
    }

    onEmployeeChange = employee => {
        var addFilter = { employee: employee };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    onGroupsChange = groups => {
        var addFilter = { groups: groups };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    addEmployeeSettings = e => {
        var groupIds = this.state.addFilter.groups.length > 0 ? this.state.addFilter.groups.map(e => e.value).toString() : null;
        var employeeId = this.state.addFilter.employee ? this.state.addFilter.employee.value : null;

        if (groupIds === null && employeeId === null) {
            toast.error('No team/employee selected');
            return;
        }

        axios({
            url: resolveClientUrl('Period/CreateEmployeeSettings'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.props.periodId,
                groupIds: groupIds,
                employeeId: employeeId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
                this.props.refreshData();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    onSelectRow = (row, isSelect, rowIndex, e) => {
        if (isSelect) {
            this.selectedIds.push(row.paRecordId);
        } else {
            this.selectedIds = this.selectedIds.filter(e => e !== row.paRecordId);
        }
    }

    onSelectAllRows = (isSelect, rows, e) => {
        if (isSelect) {
            this.selectedIds = this.props.data.map(e => e.paRecordId);
        } else {
            this.selectedIds = [];
        }
    }

    editSetting = paRecordId => {
        this.setState({ showEditModal: true, selectedIds: [paRecordId] });
    }

    editSettings = () => {
        if (this.selectedIds && this.selectedIds.length > 0) {
            this.setState({ showEditModal: true, selectedIds: this.selectedIds });
        } else {
            toast.error('No employee selected');
        }
    }

    onEditSettingStatus = () => {
        axios({
            url: resolveClientUrl('Period/EditEmployeeSettingStatus'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: this.state.selectedSettingToUpdateStatus.paRecordId,
                statusId: this.state.selectedSettingToUpdateStatus.statusId === PAStatus.Rejected ? PAStatus.NotGenerated : PAStatus.Rejected,
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
                this.setState({ showConfirmationModal: false });
                this.props.refreshData();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    closeEditModal = () => this.setState({ showEditModal: false })

    editSettingStatusConfirmation = (setting) => this.setState({ showConfirmationModal: true, selectedSettingToUpdateStatus: setting })

    onEditSuccess = () => {
        this.setState({ showEditModal: false });
        this.props.refreshData();
    }

    closeConfirmationModal = () => this.setState({ showConfirmationModal: false })

    render() {
        return (
            <Fragment>
                <ToolkitProvider keyField="paRecordId" columns={this.columns} data={this.props.data} search >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <div className="search-wrapper clearfix" >
                                        <div className="pull-left field-group">
                                            <label>Team</label>
                                            <div className="control-content">
                                                <Multiselect className="react-select-container" classNamePrefix="react-select"
                                                    value={this.state.addFilter.groups} name="employee"
                                                    options={this.props.groups}
                                                    onChange={this.onGroupsChange}
                                                />
                                            </div>
                                        </div>
                                        <div className="pull-left field-group">
                                            <label>Employee</label>
                                            <div className="control-content">
                                                <Async className="react-select-container" classNamePrefix="react-select"
                                                    cacheOptions value={this.state.addFilter.employee} name="employee"
                                                    loadOptions={this.searchEmployees} placeholder="-- Search an employee --" onChange={this.onEmployeeChange} />
                                            </div>
                                        </div>
                                        <div className="pull-left field-group">
                                            <button className="btn btn-primary" onClick={this.addEmployeeSettings}>Add</button>
                                        </div>
                                        <div className="search pull-right">
                                            <Search.SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                            <span className="fa fa-search icon" />
                                        </div>
                                    </div>
                                    <div className="search-wrapper clearfix" >
                                        <div className="pull-left btn-block">
                                            <button className="btn btn-primary" onClick={this.editSettings}>
                                                <i className="fa fa-pencil-square-o" /> Edit Employee Setting
                                                </button>
                                            <button className="btn btn-primary" onClick={this.showCreateModal}>
                                                <i className="fa fa-pencil-square-o" /> Generate PA
                                                </button>
                                        </div>
                                    </div>
                                    <BootstrapTable {...props.baseProps} noDataIndication={noDataIndication} pagination={paginationFactory(PaginationOptions)} striped
                                        selectRow={{ mode: 'checkbox', onSelect: this.onSelectRow, onSelectAll: this.onSelectAllRows }}
                                    />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
                {
                    this.state.showEditModal &&
                    <Edit showModal={this.state.showEditModal} closeModal={this.closeEditModal} paRecordIds={this.state.selectedIds} periodId={this.props.periodId}
                        settings={this.props.data.filter(e => this.state.selectedIds.some(s => s === e.paRecordId))}
                        onEditSuccess={this.onEditSuccess}
                    />
                }
                {
                    this.state.showConfirmationModal &&
                    <ConfirmationModal showModal={this.state.showConfirmationModal} closeModal={this.closeConfirmationModal}
                        action={this.state.selectedSettingToUpdateStatus.statusId === PAStatus.Rejected ? 'include' : 'exclude'}
                        name={this.state.selectedSettingToUpdateStatus.nickName}
                        handleSubmit={this.onEditSettingStatus}
                    />
                }
            </Fragment>
        );
    }
}