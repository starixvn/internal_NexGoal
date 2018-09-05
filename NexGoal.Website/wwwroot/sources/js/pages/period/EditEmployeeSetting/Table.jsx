import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { resolveClientUrl } from '../../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import Async from 'react-select/lib/Async';
import Multiselect from '../../../shared/components/MultiSelect';
import { PaginationOptions } from '../../../constants';
import commonFunctions from '../../../common-functions';

import Edit from './Edit';

function TableHead(props) {
    return (
        <thead>
            <tr>
                <th><input type="checkbox" onChange={props.selectAll} checked={props.checked} /></th>
                <th>#</th>
                <th>Team</th>
                <th>Employee</th>
                <th>Step</th>
                <th>Name</th>
                <th>Status</th>
                <th>Start Date</th>
                <th>End Date</th>
                <th>PA Status</th>
                <th>Include</th>
                <th>Action</th>
            </tr>
        </thead>);
}

function TableBody(props) {
    var body = props.data.length > 0 ? props.data.map(setting => {
        var processes = setting.processes.filter(e => e.processId !== 1)
        var rowSpan = processes.length;

        return (
            <Fragment key={setting.rowNum}>
                <tr>
                    <td className="text-center" rowSpan={rowSpan}><input type="checkbox" checked={props.selectedIds.some(e => e === setting.paRecordId)} onChange={props.onSelect} data-id={setting.paRecordId} /></td>
                    <td className="text-center" rowSpan={rowSpan}>{setting.rowNum}</td>
                    <td className="text-center" rowSpan={rowSpan}>{setting.group}</td>
                    <td rowSpan={rowSpan}>
                        <div>{setting.fullName}</div>
                        <div>{setting.code}</div>
                        <div>({setting.nickName})</div>
                        <div>{setting.jobTitle}</div>
                    </td>
                    <td>{processes[0].process}</td>
                    <td>{processes[0].evaluator}</td>
                    <td>{processes[0].status}</td>
                    <td></td>
                    <td></td>
                    <td rowSpan={rowSpan} className="text-center">{setting.status}</td>
                    <td rowSpan={rowSpan} className="text-center"><input type="checkbox" checked={setting.isIncluded} /></td>
                    <td rowSpan={rowSpan}></td>
                </tr>
                {processes.slice(1).map((process, i) =>
                    <tr key={i}>
                        <td>{process.process}</td>
                        <td>{process.evaluator}</td>
                        <td>{process.status}</td>
                        <td></td>
                        <td></td>
                    </tr>
                )}
            </Fragment>);
    }) :
        (<tr>
            <td className="text-center text-danger" colSpan="13">No information available!</td>
        </tr>);

    return (<tbody>{body}</tbody>);
}

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
            showEditModal: false
        }
    }

    selectedIds = []

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    columns = [
        {
            dataField: 'rowNum',
            text: '#',
            classes: 'text-center'
        },
        {
            dataField: 'group',
            text: 'Team'
        },
        {
            dataField: 'fullName',
            text: '',
            hidden: true
        },
        {
            dataField: 'jobTitle',
            text: '',
            hidden: true
        },
        {
            dataField: 'nickName',
            text: 'Employee',
            formatter: (cell, row, rowIndex, colIndex) => {
                return (
                    <Fragment>
                        <div>{row.fullName}</div>
                        <div>{row.code}</div>
                        <div>({row.nickName})</div>
                        <div>{row.jobTitle}</div>
                    </Fragment>
                );
            }
        },
        {
            dataField: 'evaluator',
            text: 'Evaluator',
            formatter: (cell, row, rowIndex, colIndex) => {
                var evaluatorProcess = row.processes.find(p => p.processId === 4);

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
            formatter: (cell, row, rowIndex, colIndex) => {
                var reviewerProcess = row.processes.find(p => p.processId === 5);

                return (
                    <Fragment>
                        <div>{reviewerProcess.evaluator}</div>
                        <div>{reviewerProcess.status}</div>
                    </Fragment>
                );
            }
        },
        {
            dataField: 'status',
            text: 'PA Status',
            classes: 'text-center',
        },
        {
            dataField: 'include',
            text: 'Include',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                return '';
            }
        },
        {
            dataField: '',
            text: 'Action',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                //if (this.props.hasHostPermission) {
                //    return (
                //        <Fragment>
                //            <a className="fa fa-pencil-square-o" title="Edit" href={`/period/edit/${row.periodId}`} />
                //            <a className="fa fa-trash-o remove" title="Delete" onClick={() => this.showDeleteModal(row)} />
                //        </Fragment>
                //    );
                //}

                return '';
            }
        }]

    showCreateModal = () => this.setState({ showCreateModal: true })

    closeCreateModal = () => this.setState({ showCreateModal: false })

    showDeleteModal = period => this.setState({ showDeleteModal: true, deletedPeriod: period })

    closeDeleteModal = () => this.setState({ showDeleteModal: false })

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
        var employeeId = this.state.addFilter.employee ? this.state.addFilter.employee.value : null

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
            this.selectedIds.push(row.paRecordId)
        } else {
            this.selectedIds = this.selectedIds.filter(e => e !== row.paRecordId);
        }
    }

    onSelectAllRows = (isSelect, rows, e) => {
        if (isSelect) {
            this.selectedIds = this.props.data.map(e => e.paRecordId)
        } else {
            this.selectedIds = [];
        }
    }

    editSettings = () => {
        this.setState({ showEditModal: true });
    }

    closeEditModal = () => this.setState({ showEditModal: false })

    render() {
        return (
            <Fragment>

                <div className="search-wrapper clearfix" >
                    <div className="pull-left btn-block">
                        <button className="btn btn-primary" onClick={this.editSettings}><i className="fa fa-pencil-square-o"></i> Edit Employee Setting</button>
                        <button className="btn btn-primary" onClick={this.showCreateModal}><i className="fa fa-pencil-square-o"></i> Generate PA</button>
                    </div>
                </div>
                <ToolkitProvider keyField="paRecordId" data={this.props.data} columns={this.columns} search >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <div className="search-wrapper clearfix" >
                                        <div className="pull-left field-group">
                                            <label>Department/Team</label>
                                            <div className="control-content">
                                                <Multiselect className="react-select-container" classNamePrefix="react-select"
                                                    value={this.state.addFilter.groups} name="employee" options={this.props.groups} onChange={this.onGroupsChange} />
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
                                            <span className="fa fa-search icon"></span>
                                        </div>
                                    </div>
                                    <BootstrapTable {...props.baseProps} noDataIndication={this.noDataDisplay} pagination={paginationFactory(PaginationOptions)} striped
                                        selectRow={{ mode: 'checkbox', onSelect: this.onSelectRow, onSelectAll: this.onSelectAllRows }}
                                    />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
                {this.state.showEditModal && <Edit showModal={this.state.showEditModal} closeModal={this.closeEditModal} paRecordIds={this.selectedIds} settings={this.props.data.filter(e => this.selectedIds.some(s => s === e.paRecordId))} />}
            </Fragment>
        );
    }
}