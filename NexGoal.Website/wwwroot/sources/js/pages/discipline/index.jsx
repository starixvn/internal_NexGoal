import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Notification from '../../shared/components/notification';
import { Async } from 'react-select';

export default class DisciplineManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                from: moment().startOf('year'),
                to: moment(),
                isActive: '',
                groupIds: '',
                employee: ''
            },
            data: [],
            tableData: []
        };

        this.tableRef = React.createRef();
        this.searchRef = React.createRef();
        this.notificationRef = React.createRef();
    }

    handleFromDateChange = (date) => {
        this.updateFilterValue('from', date);
    }

    handleToDateChange = (date) => {
        this.updateFilterValue('to', date);
    }

    onFilterChange = (e) => {
        this.updateFilterValue(e.target.name, e.target.value)
    }

    onChangeRaw = (e) => {
        let date = moment(e.target.value);
        if (date.isValid()) {
            this.updateFilterValue(e.target.name, date);
        }
    }

    handleEmployeeChange = (selectedOption) => {
        this.updateFilterValue('employee', selectedOption);
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    searchEmployees = (input, callback) => {
        if (input.length > 2) {
            return fetch(`/common/SearchEmployees?keyword=${input}`,
                {
                    credentials: "same-origin"
                })
                .then((response) => {
                    return response.json();
                }).then(data => {
                    let options = [];

                    data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: `${item.fullName} (${item.nickName})`
                        });
                    });
                    return { options };
                });
        }

        callback(null, { options: [] });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Incident Management</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Notification ref={this.notificationRef} />
                    <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                        <div className="form-group" >
                            <div className="col" >
                                <label className="control-label">From</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.from} onChange={this.handleFromDateChange} onChangeRaw={this.onChangeRaw}
                                        name="from" className="form-control" showMonthDropdown showYearDropdown />
                                </div>
                            </div>
                            <div className="col" >
                                <label className="control-label">To</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.to} onChange={this.handleToDateChange} onChangeRaw={this.onChangeRaw}
                                        showMonthDropdown showYearDropdown name="to" className="form-control" />
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">Status</label>
                                <div className="control-content">
                                    <select name="isActive" className="form-control" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        <option value="true">Active</option>
                                        <option value="false">Inactive</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col">
                                <label className="control-label">Team</label>
                                <div className="control-content">
                                    <select className="form-control" name="groupIds" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        <option value="false">Expat</option>
                                        <option value="true">Local</option>
                                    </select>
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">Employee</label>
                                <div className="control-content">
                                    <Async autoload={false} value={this.state.filter.employee} name="employee" onChange={this.handleEmployeeChange} loadOptions={this.searchEmployees} placeholder="-- Search an employee --" />
                                </div>
                            </div>
                            <div className="col">
                                <button className="btn btn-primary">Search</button>
                            </div>
                        </div>
                    </form>
                    <div className="search-wrapper clearfix" >
                        <div className="pull-left">
                            <button className="btn btn-primary"><i className="fa fa-plus"></i> Add Incident Record</button>
                        </div>
                        <div className="search pull-right">
                            <input className="search-textbox" ref={this.searchRef} onChange={this.onTableFilterChange} />
                            <span className="fa fa-search icon"></span>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<DisciplineManagement />, document.getElementById("discipline-wrapper"));