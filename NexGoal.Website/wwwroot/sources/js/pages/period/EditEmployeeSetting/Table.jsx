import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import commonFunctions from '../../../common-functions';
import { toast } from 'react-toastify';
import { range } from 'lodash';
import { Pagination, DropdownButton, MenuItem } from 'react-bootstrap';
import Async from 'react-select/lib/Async';
import memoize from "memoize-one";
import Multiselect from '../../../shared/components/MultiSelect';

function TableHead(props) {
    debugger;
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
        var rowSpan = setting.processes.length;

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
                    <td>{setting.processes[0].process}</td>
                    <td>{setting.processes[0].evaluator}</td>
                    <td>{setting.processes[0].status}</td>
                    <td></td>
                    <td></td>
                    <td rowSpan={rowSpan} className="text-center">{setting.status}</td>
                    <td rowSpan={rowSpan} className="text-center"><input type="checkbox" checked={setting.isIncluded} /></td>
                    <td rowSpan={rowSpan}></td>
                </tr>
                {setting.processes.slice(1).map((process, i) =>
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
            filterText: '',
            page: 1,
            pageSize: 20,
            addFilter: {
                groups: [],
                employee: null
            },
            selectedIds: [],
            showEditModal: false
        }
    }

    onPageSizeChange = e => {
        e.preventDefault();

        var pageSize = Number.parseInt(e.target.dataset.page);

        this.setState({
            pageSize: pageSize,
            page: 1,
        });
    }

    onPageChange = page => {
        this.setState({
            page: page,
        });
    }

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

    filterData = memoize((data, filterText) => {
        return data.filter(e => e.nickName.includes(filterText) || e.fullName.includes(filterText));
    })

    getTableData = memoize((data, page, pageSize) => {
        var from = (page - 1) * pageSize;

        return data.slice(from, from + pageSize);
    })

    onEmployeeChange = employee => {
        var addFilter = { employee: employee };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    onGroupsChange = groups => {
        var addFilter = { groups: groups };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    onFilterChange = e => this.setState({ filterText: e.target.value })

    addEmployeeSettings = e => {
        var groupIds = this.state.addFilter.groups.length > 0 ? this.state.addFilter.groups.map(e => e.value).toString() : null;
        var employeeId = this.state.addFilter.employee ? this.state.addFilter.employee.value : null

        if (groupIds === null && employeeId === null) {
            toast.error('No team/employee selected');
            return;
        }

        axios({
            url: '/Period/CreateEmployeeSettings',
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
        }).catch(commonFunctions.axiosError);
    }

    onPASelect = e => {
        var id = parseInt(e.target.dataset.id);
        var selectedIds = this.state.selectedIds.slice(0);

        if (e.target.checked) {
            selectedIds.push(id)
        } else {
            selectedIds = selectedIds.filter(e => e !== id);
        }

        this.setState({ selectedIds: selectedIds });
    }

    onSelectAll = e => {
        if (e.target.checked) {
            this.setState({ selectedIds: this.props.data.map(e => e.paRecordId) })
        } else {
            this.setState({ selectedIds: [] })
        }
    }

    render() {
        var data = this.filterData(this.props.data, this.state.filterText);
        var totalPage = Math.ceil(data.length / this.state.pageSize);

        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left field-group">
                        <label>Department/Team</label>
                        <div className="control-content">
                            <Multiselect className="react-select-container" classNamePrefix="react-select"
                                value={this.state.addFilter.groups}
                                name="employee"
                                options={this.props.groups}
                                onChange={this.onGroupsChange} />
                        </div>
                    </div>
                    <div className="pull-left field-group">
                        <label>Employee</label>
                        <div className="control-content">
                            <Async className="react-select-container" classNamePrefix="react-select"
                                cacheOptions value={this.state.addFilter.employee}
                                name="employee"
                                loadOptions={this.searchEmployees}
                                placeholder="-- Search an employee --"
                                onChange={this.onEmployeeChange} />
                        </div>
                    </div>
                    <div className="pull-left field-group">
                        <button className="btn btn-primary" onClick={this.addEmployeeSettings}>Add</button>
                    </div>
                    <div className="search pull-right">
                        <input className="search-textbox" onChange={this.onFilterChange} />
                        <span className="fa fa-search icon"></span>
                    </div>
                </div>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left btn-block">
                        <button className="btn btn-primary" onClick={this.showCreateModal}><i className="fa fa-pencil-square-o"></i> Edit Employee Setting</button>
                        <button className="btn btn-primary" onClick={this.showCreateModal}><i className="fa fa-pencil-square-o"></i> Generate PA</button>
                    </div>
                </div>
                <table className="table table-bordered table-striped">
                    <TableHead checked={this.state.selectedIds.length === this.props.data.length} selectAll={this.onSelectAll} />
                    <TableBody data={this.getTableData(data, this.state.page, this.state.pageSize)} selectedIds={this.state.selectedIds} onSelect={this.onPASelect} />
                </table>

                <div className="clearfix">
                    <div className="pull-left page-size">
                        <DropdownButton
                            bsStyle="default"
                            title={this.state.pageSize}
                            id="page-size-select"
                        >
                            <MenuItem onClick={this.onPageSizeChange} data-page="20">20</MenuItem>
                            <MenuItem onClick={this.onPageSizeChange} data-page="50">50</MenuItem>
                            <MenuItem onClick={this.onPageSizeChange} data-page="100">100</MenuItem>
                            <MenuItem onClick={this.onPageSizeChange} data-page="200">200</MenuItem>
                        </DropdownButton>
                    </div>
                    <div className="pull-right">
                        <Pagination>
                            {this.state.page > 1 && <Pagination.First onClick={e => this.onPageChange(1)} />}
                            {this.state.page > 1 && <Pagination.Prev onClick={e => this.onPageChange(this.state.page - 1)} />}
                            {range(1, totalPage + 1).map(page =>
                                <Pagination.Item key={page} active={this.state.page === page} onClick={e => this.onPageChange(page)} >{page}</Pagination.Item>
                            )}
                            {this.state.page < totalPage && <Pagination.Next onClick={e => this.onPageChange(this.state.page + 1)} />}
                            {this.state.page < totalPage && <Pagination.Last onClick={e => this.onPageChange(totalPage)} />}
                        </Pagination>
                    </div>
                </div>
            </Fragment>
        );
    }
}