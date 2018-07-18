import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Notification from '../../shared/components/notification';
import { Async } from 'react-select';
import Table from './Table';
import EditDiscipline from './EditDiscipline';

export default class DisciplineManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                from: moment().startOf('year'),
                to: moment(),
                isActive: '',
                groupIds: [],
                employee: '',
            },
            data: [],
            tableData: [],
            canEdit: document.getElementById('CanEdit').value === 'True',
            showModal: false
        };

        this.tableRef = React.createRef();
        this.searchRef = React.createRef();

        this.notificationRef = React.createRef();
    }

    componentDidMount = function () {
        this.getDisciplines();
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.searchRef.current.value = '';
        this.getDisciplines();
    }

    onFromDateChange = (date) => {
        this.updateFilterValue('from', date);
    }

    onToDateChange = (date) => {
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

    onEmployeeChange = (selectedOption) => {
        this.updateFilterValue('employee', selectedOption);
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    getDisciplines = () => {
        var employeeId = this.state.filter.employee ? this.state.filter.employee.employeeId : '';

        axios(`/Discipline/GetDisciplines?from=${this.state.filter.from.format('YYYY-MM-DD')}&to=${this.state.filter.to.format('YYYY-MM-DD')}&isActive=${this.state.filter.isActive}&groupIds=${this.state.filter.groupIds.toString()}&employeeId=${employeeId}`)
            .then(response => {
                response.data.map((e, index) => e.rowNum = index + 1);
                this.setState({ data: response.data, tableData: response.data });
            });
    }

    onTableFilterChange = (e) => {
        var filterData = this.getTableFilteredData(e.target.value, this.state.data);
        this.setState({ tableData: filterData });
    }

    getTableFilteredData = (searchValue, data) => {
        if (searchValue && searchValue.length > 1) {
            var value = searchValue.trim().toLowerCase();
            var filterData = data.filter((discipline) =>
                discipline.nickName.toLowerCase().indexOf(value) >= 0
                || discipline.fullName.toLowerCase().indexOf(value) >= 0
                || discipline.group.toLowerCase().indexOf(value) >= 0
                || discipline.description.toLowerCase().indexOf(value) >= 0
                || discipline.creatorFullName.toLowerCase().indexOf(value) >= 0
                || discipline.creatorNickName.toLowerCase().indexOf(value) >= 0
            );
            return filterData;
        } else {
            return data.slice(0);
        }
    }

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

    onUpdateStatus = (disciplineId, isActive, response) => {
        if (response.isSuccessful) {
            this.notificationRef.current.showSuccess('Update Successfully');
            var data = this.state.data.slice();
            var discipline = data.find(e => e.disciplineId === disciplineId);
            discipline.isActive = isActive;

            this.setState({ data: data })
        } else {
            this.notificationRef.current.showError(response.message);
        }
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    showModal = () => {
        this.setState({ showModal: true });
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
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.from} onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                                        name="from" className="form-control" showMonthDropdown showYearDropdown />
                                </div>
                            </div>
                            <div className="col" >
                                <label className="control-label">To</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.to} onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
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
                                    <Async valueKey="employeeId" labelKey="name" autoload={false} value={this.state.filter.employee} name="employee" onChange={this.onEmployeeChange} loadOptions={this.searchEmployees} placeholder="-- Search an employee --" />
                                </div>
                            </div>
                            <div className="col">
                                <button className="btn btn-primary">Search</button>
                            </div>
                        </div>
                    </form>
                    <div className="search-wrapper clearfix" >
                        <div className="pull-left">
                            {this.state.canEdit &&
                                <button className="btn btn-primary" onClick={this.showModal}><i className="fa fa-plus"></i> Add Incident Record</button>
                            }
                        </div>
                        <div className="search pull-right">
                            <input className="search-textbox" ref={this.searchRef} onChange={this.onTableFilterChange} />
                            <span className="fa fa-search icon"></span>
                        </div>
                    </div>
                    <Table data={this.state.tableData} onUpdateStatus={this.onUpdateStatus} canEdit={this.state.canEdit} />
                </div>
                {this.state.showModal &&
                    <EditDiscipline showModal={this.state.showModal} closeModal={this.closeModal} />
                }
            </div>
        );
    }
}

ReactDOM.render(<DisciplineManagement />, document.getElementById("discipline-wrapper"));