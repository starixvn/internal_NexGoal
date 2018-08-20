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
import { Async } from 'react-select';

export default class Table extends Component {
    constructor(props) {
        super(props);

        let data = range(1, 100).map(e => { return { rowNum: e, group: 'BA' } });

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            data: data,
            tableData: data,
            page: 1,
            pageSize: 20,
            totalPage: Math.ceil(data.length / 20)
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

    editPeriodStatus = (event, period) => {
        var isActive = event.target.checked;

        axios({
            url: '/period/editstatus',
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: period.periodId,
                isActive: isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
                this.props.onUpdateStatus(period.periodId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);;
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
                            employeeId: item.employeeId,
                            name: `${item.fullName} (${item.nickName})`
                        });
                    });

                    return { options };
                });
        }

        callback(null, { options: [] });
    }

    render() {
        var from = (this.state.page - 1) * this.state.pageSize;

        var tableBody = this.state.tableData.length > 0 ? this.state.tableData.slice(from, from + this.state.pageSize).map(e =>
            <Fragment key={e.rowNum}>
                <tr>
                    <td className="text-center" rowSpan="4"><input type="checkbox" /></td>
                    <td className="text-center" rowSpan="4">{e.rowNum}</td>
                    <td className="text-center" rowSpan="4">{e.group}</td>
                    <td rowSpan="4"><div>12313</div><div>12313</div></td>
                    <td className="text-center" rowSpan="4">{e.targetType}</td>
                    <td>Self-Evaluator</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Project Manager</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Evaluator</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Reviewer</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
            </Fragment>) :
            <tr>
                <td className="text-center text-danger" colspan="13">No information available!</td>
            </tr>

        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left field-group">
                        <label>Employee</label>
                        <div className="control-content">
                            <Async valueKey="employeeId" labelKey="name" autoload={false} value={null}
                                name="employee" onChange={this.onFilterEmployeeChange}
                                loadOptions={this.searchEmployees}
                                placeholder="-- Search an employee --" />
                        </div>
                    </div>
                    <div className="pull-left field-group">
                        <button className="btn btn-primary">Add</button>
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
                    <thead>
                        <tr>
                            <th><input type="checkbox" /></th>
                            <th>#</th>
                            <th>Department / Team</th>
                            <th>Employee</th>
                            <th>Target Type</th>
                            <th>Step</th>
                            <th>Name</th>
                            <th>Status</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Is PA Generated</th>
                            <th>Include</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>{tableBody}</tbody>
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