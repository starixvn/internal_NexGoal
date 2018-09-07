import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { resolveClientUrl } from '../../common-functions';
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
                isActive: '',
                groups: [],
                employee: null
            },
            data: [],
            groupOptions: [],
            isHost: document.getElementById('IsHost').checked,
            showModal: false
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleFilterSubmit = this.handleFilterSubmit.bind(this);
    }

    componentDidMount = function () {
        this.getGroups();
        this.getDisciplines();
    }

    handleFilterChange(name, value) {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit() {
        this.getDisciplines();
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
                    <SearchForm filter={this.state.filter}
                        groupOptions={this.state.groupOptions}
                        searchEmployees={this.searchEmployees}
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