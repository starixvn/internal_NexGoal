import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';

import SearchForm from './SearchForm';
import Table from './Table';
import AddEditDiscipline from './EditDiscipline';

export default class DisciplineManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                from: moment().startOf('year'),
                to: moment(),
                isActive: { value: '', label: '-- All --' },
                groups: [],
                employee: null
            },
            data: [],
            groupOptions: [],
            employeeOptions: [],
            isHost: document.getElementById('IsHost').checked,
            showModal: false
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleFilterSubmit = this.handleFilterSubmit.bind(this);
    }

    componentDidMount = function () {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse]) => {
                let employeeOptions = [];
                const groupOptions = groupResponse.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                employeeResponse.data.map(item => {
                    employeeOptions.push({
                        value: item.employeeId,
                        label: `${item.fullName} (${item.nickName})`
                    });
                });

                const nextState = Object.assign({}, this.state.filter, { groups: groupOptions });

                this.setState({
                    groupOptions: groupOptions,
                    employeeOptions: employeeOptions,
                    filter: nextState
                });

                this.getDisciplines(nextState);
            }).catch(axiosError);
    }

    handleFilterChange(name, value) {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit() {
        this.getDisciplines(this.state.filter);
    }

    getDisciplines = (filter) => {
        const employeeId = filter.employee ? filter.employee.value : '';
        const groupIds = filter.groups.map((group) => { return group.value; }).join(',');

        axios(resolveClientUrl('Discipline/GetDisciplines'), {
            params: {
                fromDate: filter.from.format('YYYY-MM-DD'),
                toDate: filter.to.format('YYYY-MM-DD'),
                isActive: filter.isActive ? filter.isActive.value : null,
                employeeId: employeeId,
                groupIds: groupIds
            }
        }
        ).then(response => {
            this.setState({ data: response.data });
        });
    }

    onUpdateStatus = (disciplineId, isActive) => {
        var data = this.state.data.slice();
        var discipline = data.find(e => e.disciplineId === disciplineId);
        discipline.isActive = isActive;
        this.setState({ data: data });
    }

    onEditDiscipline = () => {
        this.closeModal();
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
                    <SearchForm filter={this.state.filter}
                        groupOptions={this.state.groupOptions}
                        employeeOptions={this.state.employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.handleFilterSubmit}
                    />

                    <Table data={this.state.data} onUpdateStatus={this.onUpdateStatus}
                        isHost={this.state.isHost}
                        addDiscipline={this.addDiscipline}
                        editDiscipline={this.editDiscipline}
                    />
                </div>
                <AddEditDiscipline
                    employeeOptions={this.state.employeeOptions}
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