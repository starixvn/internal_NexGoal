import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Multiselect from '../../shared/components/MultiSelect';
import { Async } from 'react-select';
import Table from './Table';
import AddEditDiscipline from './EditDiscipline';
import ToastLayout from '../../shared/layouts/ToastLayout';

export default class DisciplineManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                from: moment().startOf('year'),
                to: moment(),
                isActive: '',
                groupIds: '',
                employee: '',
            },
            data: [],
            tableData: [],
            groups: [],
            canEdit: document.getElementById('CanEdit').value === 'True',
            showModal: false
        };
        this.getGroups();
        this.tableRef = React.createRef();
        this.searchRef = React.createRef();
    }

    componentDidMount = function () {
        this.getDisciplines();
    }

    handleSubmit = (event) => {
        event.preventDefault();
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

    onFilterEmployeeChange = (selectedOption) => {
        this.updateFilterValue('employee', selectedOption);
    }

    onFilterGroupChange = (selectedOptions) => {
        this.updateFilterValue('groupIds', selectedOptions);
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    getDisciplines = () => {
        var employeeId = this.state.filter.employee ? this.state.filter.employee.employeeId : '';

        axios(`/Discipline/GetDisciplines?from=${this.state.filter.from.format('YYYY-MM-DD')}&to=${this.state.filter.to.format('YYYY-MM-DD')}&isActive=${this.state.filter.isActive}&employeeId=${employeeId}&groupIds=${this.state.filter.groupIds}`)
            .then(response => {
                response.data.forEach((e, index) => e.rowNum = index + 1);
                this.setState({ data: response.data, tableData: response.data });
            });
    }

    getGroups = () => {
        axios('/Common/GetGroups')
            .then(response => {
                this.setState({
                    groups: response.data,
                    filter: {
                        ...this.state.filter,
                        ...{ groupIds: response.data.map(g => g.groupId).toString() }
                    }
                });
            });
    }

    renderValue = (option) => {
        return (<span>{this.state.filter.groupIds.split(',').length} selected</span >);
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

    onUpdateStatus = (disciplineId, isActive) => {
        var data = this.state.data.slice();
        var discipline = data.find(e => e.disciplineId === disciplineId);
        discipline.isActive = isActive;
        this.setState({ data: data })
    }

    onEditDiscipline = (disciplineId, response) => {
        this.getDisciplines();
    }

    editDiscipline = (discipline) => {
        this.setState({
            showModal: true,
            selectedDiscipline: {
                disciplineId: discipline.disciplineId,
                employee: {
                    employeeId: discipline.employeeId,
                    name: `${discipline.fullName} (${discipline.nickName})`
                },
                effectiveDate: moment(discipline.effectiveDate),
                content: discipline.description
            }
        });
    }

    addDiscipline = () => {
        this.setState({
            showModal: true,
            selectedDiscipline: { disciplineId: 0, employee: null, effectiveDate: moment(), content: '' }
        });
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
                    <li className="current">Discipline Management</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                        <div className="form-group" >
                            <div className="col" >
                                <label className="control-label">From</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.from}
                                        onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                                        name="from" className="form-control" autoComplete="off" />
                                </div>
                            </div>
                            <div className="col" >
                                <label className="control-label">To</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.to}
                                        onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                                        name="to" className="form-control" autoComplete="off" />
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
                                    {this.state.groups.length > 0 &&
                                        <Multiselect value={this.state.filter.groupIds} simpleValue={true} valueKey="groupId" labelKey="name"
                                            options={this.state.groups}
                                            onChange={this.onFilterGroupChange}
                                            removeSelected={false} closeOnSelect={false} />
                                    }
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">Employee</label>
                                <div className="control-content">
                                    <Async valueKey="employeeId" labelKey="name" autoload={false} value={this.state.filter.employee}
                                        name="employee" onChange={this.onFilterEmployeeChange}
                                        loadOptions={this.searchEmployees}
                                        placeholder="-- Search an employee --" />
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
                                <button className="btn btn-primary" onClick={this.addDiscipline}><i className="fa fa-plus"></i> Add Discipline</button>
                            }
                        </div>
                        <div className="search pull-right">
                            <input className="search-textbox" ref={this.searchRef} onChange={this.onTableFilterChange} />
                            <span className="fa fa-search icon"></span>
                        </div>
                    </div>
                    <Table data={this.state.tableData} onUpdateStatus={this.onUpdateStatus} canEdit={this.state.canEdit} editDiscipline={this.editDiscipline} />
                </div>
                <AddEditDiscipline ref={this.modalRef} showModal={this.state.showModal} closeModal={this.closeModal} discipline={this.state.selectedDiscipline} onEditDiscipline={this.onEditDiscipline} />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<DisciplineManagement />, document.getElementById("discipline-wrapper"));