import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { Async } from 'react-select';

import { resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Multiselect from '../../shared/components/MultiSelect';
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
                groups: [],
                employee: null
            },
            data: [],
            groupOptions: [],
            isHost: document.getElementById('IsHost').checked,
            showModal: false
        };
    }

    componentDidMount = function () {
        this.getGroups();
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
        this.updateFilterValue(e.target.name, e.target.value);
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
        this.updateFilterValue('groups', selectedOptions);
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    getDisciplines = () => {
        const employeeId = this.state.filter.employee ? this.state.filter.employee.value : '';
        const groupIds = this.state.filter.groups.map((group) => { return group.value; }).join(',');
        const filter = this.state.filter;

        axios(resolveClientUrl('Discipline/GetDisciplines'), {
            params: {
                from: filter.from.format('YYYY-MM-DD'),
                to: filter.to.format('YYYY-MM-DD'),
                isActive: filter.isActive,
                employeeId: employeeId,
                groupIds: groupIds
            }
        }
        ).then(response => {
            response.data.forEach((e, index) => e.rowNum = index + 1);
            this.setState({ data: response.data });
        });
    }

    getGroups = () => {
        axios(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                const groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                this.setState({
                    groupOptions: groupOptions,
                    filter: Object.assign({}, this.state.filter, { groups: groupOptions })
                });
            });
    }

    searchEmployees = (input, callback) => {
        if (input.length > 1) {
            return axios(resolveClientUrl(`common/SearchEmployees?keyword=${input}`))
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: `${item.fullName} (${item.nickName})`
                        });
                    });

                    return options;
                });
        }

        this.setState({ filter: Object.assign({}, this.state.filter, { employee: null }) });
        return [];
    }

    onUpdateStatus = (disciplineId, isActive) => {
        var data = this.state.data.slice();
        var discipline = data.find(e => e.disciplineId === disciplineId);
        discipline.isActive = isActive;
        this.setState({ data: data });
    }

    onEditDiscipline = () => {
        this.getDisciplines();
    }

    editDiscipline = (discipline) => {
        this.setState({
            showModal: true,
            selectedDiscipline: {
                disciplineId: discipline.disciplineId,
                employee: {
                    value: discipline.employeeId,
                    label: `${discipline.fullName} (${discipline.nickName})`
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
                    <li className="current">{getResource('DisciplineManagement')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                        <div className="form-group" >
                            <div className="col" >
                                <label className="control-label">{getResource('From')}</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.from}
                                        onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                                        name="from" className="form-control" autoComplete="off"
                                    />
                                </div>
                            </div>
                            <div className="col" >
                                <label className="control-label">{getResource('To')}</label>
                                <div className="control-content">
                                    <DatePicker dateFormat="DD MMM, YYYY" selected={this.state.filter.to}
                                        onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                                        name="to" className="form-control" autoComplete="off"
                                    />
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">{getResource('Status')}</label>
                                <div className="control-content">
                                    <select name="isActive" className="form-control" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        <option value="true">{getResource('Active')}</option>
                                        <option value="false">{getResource('Inactive')}</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col">
                                <label className="control-label">{getResource('Team')}</label>
                                <div className="control-content">
                                    {this.state.groupOptions.length > 0 &&
                                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                                            value={this.state.filter.groups}
                                            closeMenuOnSelect={false}
                                            options={this.state.groupOptions}
                                            onChange={this.onFilterGroupChange}
                                        />
                                    }
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">{getResource('Employee')}</label>
                                <div className="control-content">
                                    <Async
                                        className="react-select-container" classNamePrefix="react-select"
                                        cacheOptions value={this.state.filter.employee}
                                        name="employee" onChange={this.onFilterEmployeeChange}
                                        loadOptions={this.searchEmployees}
                                        placeholder="-- Search an employee --"
                                    />
                                </div>
                            </div>
                            <div className="col">
                                <button className="btn btn-primary">{getResource('Search')}</button>
                            </div>
                        </div>
                    </form>
                    <Table data={this.state.data} onUpdateStatus={this.onUpdateStatus}
                        isHost={this.state.isHost}
                        addDiscipline={this.addDiscipline}
                        editDiscipline={this.editDiscipline}
                    />
                </div>
                <AddEditDiscipline
                    showModal={this.state.showModal}
                    closeModal={this.closeModal}
                    discipline={this.state.selectedDiscipline}
                    onEditDiscipline={this.onEditDiscipline}
                />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<DisciplineManagement />, document.getElementById("discipline-wrapper"));