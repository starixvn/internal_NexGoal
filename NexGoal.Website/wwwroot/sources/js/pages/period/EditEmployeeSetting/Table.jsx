import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import commonFunctions from '../../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import { range } from 'lodash';
import { Pagination, DropdownButton, MenuItem } from 'react-bootstrap';
import Async from 'react-select/lib/Async';
import memoize from "memoize-one";
import Multiselect from '../../../shared/components/MultiSelect';

function TableHead(props) {
    return (
        <thead>
            <tr>
                <th><input type="checkbox" /></th>
                <th>#</th>
                <th>Department / Team</th>
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
        var rowSpan = 0;
        for (let i = 0; i < setting.processes.length; i++) {
            rowSpan += setting.processes[i].evaluators.length;
        }

        return (
            <Fragment key={setting.rowNum}>
                <tr>
                    <td className="text-center" rowSpan={rowSpan}><input type="checkbox" /></td>
                    <td className="text-center" rowSpan={rowSpan}>{setting.rowNum}</td>
                    <td className="text-center" rowSpan={rowSpan}>{setting.group}</td>
                    <td rowSpan={rowSpan}>
                        <div>{setting.fullName}</div>
                        <div>{setting.code}</div>
                        <div>({setting.nickName})</div>
                        <div>{setting.jobTitle}</div>
                    </td>
                    <td>{setting.flowSettings[0].process}</td>
                    <td>{setting.flowSettings[0].evaluators[0].evaluator}</td>
                    <td></td>
                    <td>{moment(setting.flowSettings[0].startDate).format('DD MMM, YYYY')}</td>
                    <td>{moment(setting.flowSettings[0].endDate).format('DD MMM, YYYY')}</td>
                    <td rowSpan={rowSpan}>{setting.status}</td>
                    <td rowSpan={rowSpan}>{setting.isIncluded}</td>
                    <td rowSpan={rowSpan}></td>
                </tr>
                {setting.flowSettings.slice(1).map(flowSetting =>
                    <Fragment key={flowSetting.flowSettingId}>
                        <tr>
                            <td rowSpan={flowSetting.evaluators.length}>{flowSetting.process}</td>
                            <td>{flowSetting.evaluators[0].evaluator}</td>
                            <td></td>
                            <td rowSpan={flowSetting.evaluators.length}>{moment(flowSetting.startDate).format('DD MMM, YYYY')}</td>
                            <td rowSpan={flowSetting.evaluators.length}>{moment(flowSetting.endDate).format('DD MMM, YYYY')}</td>
                        </tr>
                        {flowSetting.evaluators.slice(1).map((evaluator, i) =>
                            <tr key={i}>
                                <td>{evaluator.evaluator}</td>
                                <td></td>
                            </tr>)
                        }
                    </Fragment>
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
            page: 1,
            pageSize: 20,
            totalPage: Math.ceil(this.props.data.length / 20),
            addFilter: {
                groups: [],
                employee: null
            }
        }
        this.tableRef = React.createRef();
    }

    handlePageSizeChange = e => {
        e.preventDefault();

        var pageSize = Number.parseInt(e.target.dataset.page);
        var totalPage = Math.ceil(this.state.data.length / pageSize);
        var page = this.state.page <= totalPage ? this.state.page : totalPage;

        this.setState({
            pageSize: pageSize,
            totalPage: totalPage,
            page: page,
        });
    }

    handlePageChange = page => {
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

    filterData = memoize((data) => {
        var from = (this.state.page - 1) * this.state.pageSize;
        return data.slice(from, from + this.state.pageSize);
    })

    handleEmployeeChange = employee => {
        var addFilter = { employee: employee };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    handleGroupsChange = groups => {
        var addFilter = { groups: groups };
        this.setState({ addFilter: { ...this.state.addFilter, ...addFilter } });
    }

    addEmployeeSettings = e => {
        axios({
            url: '/Period/CreateEmployeeSettings',
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.props.periodId,
                groupIds: this.state.addFilter.groups ? this.state.addFilter.groups.map(e => e.value).toString() : null,
                employeeId: this.state.addFilter.employee ? this.state.addFilter.employee.value : null
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left field-group">
                        <label>Department/Team</label>
                        <div className="control-content">
                            <Multiselect value={this.state.addFilter.groups}
                                name="employee"
                                options={this.props.groups}
                                onChange={this.handleGroupsChange} />
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
                                onChange={this.handleEmployeeChange} />
                        </div>
                    </div>
                    <div className="pull-left field-group">
                        <button className="btn btn-primary" onClick={this.addEmployeeSettings}>Add</button>
                    </div>
                    <div className="search pull-right">
                        <input className="search-textbox" ref={this.searchRef} onChange={this.handleTableFilterChange} />
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
                    <TableHead />
                    <TableBody data={[]} />
                </table>

                <div className="clearfix">
                    <div className="pull-left page-size">
                        <DropdownButton
                            bsStyle="default"
                            title={this.state.pageSize}
                            id="page-size-select"
                        >
                            <MenuItem onClick={this.handlePageSizeChange} data-page="20">20</MenuItem>
                            <MenuItem onClick={this.handlePageSizeChange} data-page="50">50</MenuItem>
                            <MenuItem onClick={this.handlePageSizeChange} data-page="100">100</MenuItem>
                            <MenuItem onClick={this.handlePageSizeChange} data-page="200">200</MenuItem>
                        </DropdownButton>
                    </div>
                    <div className="pull-right">
                        <Pagination>
                            {this.state.page > 1 && <Pagination.First onClick={e => this.handlePageChange(1)} />}
                            {this.state.page > 1 && <Pagination.Prev onClick={e => this.handlePageChange(this.state.page - 1)} />}
                            {range(1, this.state.totalPage + 1).map(page =>
                                <Pagination.Item key={page} active={this.state.page === page} onClick={e => this.handlePageChange(page)} >{page}</Pagination.Item>
                            )}
                            {this.state.page < this.state.totalPage && <Pagination.Next onClick={e => this.handlePageChange(this.state.page + 1)} />}
                            {this.state.page < this.state.totalPage && <Pagination.Last onClick={e => this.handlePageChange(this.state.totalPage)} />}
                        </Pagination>
                    </div>
                </div>
            </Fragment>
        );
    }
}