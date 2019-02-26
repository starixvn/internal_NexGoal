import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { StatusOptions } from '../../constants';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';

import SearchForm from './SearchForm';
import Table from './Table';
import Create from './Create';
import Edit from './Edit';

export default class Discipline extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                from: moment().startOf('year'),
                to: moment(),
                isActive: StatusOptions[1],
                group: { value: '', label: '-- All --' },
                employee: null
            },
            disciplines: [],
            showCreateModal: false,
            editDiscipline: {
                showModal: false,
                discipline: null
            }
        };

        this.isHost = document.getElementById('IsHost').checked;
        this.groupOptions = [];
        this.employeeOptions = [];
        this.allEmployeeOptions = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse]) => {
                let groupOptions = groupResponse.data.map((group) => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });
                groupOptions.unshift({ value: '', label: '-- All --' });

                let employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                this.groupOptions = groupOptions;
                this.employeeOptions = employeeOptions;
                this.allEmployeeOptions = employeeOptions;

                this.getDisciplines(this.state.filter);
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        }, () => {
            if (name === 'group') {
                this.getEmployees(value ? value.value : null);
            }
        });
    }

    handleFilterSubmit = () => {
        this.getDisciplines(this.state.filter);
    }

    getEmployees = (groupIds) => {
        axios(resolveClientUrl('common/GetEmployees'), { params: { groupIds: groupIds } })
            .then(response => {
                let employeeOptions = response.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });
                let filter = this.state.filter;

                if (employeeOptions.indexOf(filter.employee) < 0) {
                    filter = Object.assign({}, this.state.filter, { employee: null });
                }

                this.employeeOptions = employeeOptions;

                this.setState({
                    filter: filter
                });
            }).catch(axiosError);
    }

    getDisciplines = (filter) => {
        axios(resolveClientUrl('Discipline/GetDisciplines'), {
            params: {
                fromDate: filter.from.format('YYYY-MM-DD'),
                toDate: filter.to.format('YYYY-MM-DD'),
                isActive: filter.isActive ? filter.isActive.value : null,
                groupId: filter.group ? filter.group.value : null,
                employeeId: filter.employee ? filter.employee.value : null
            }
        }).then(response => {
            this.setState({ disciplines: response.data });
        }).catch(axiosError);
    }

    handleClickCreate = () => {
        this.setState({ showCreateModal: true });
    }

    handleCancelCreate = () => {
        this.setState({ showCreateModal: false });
    }

    handleCreateSuccess = () => {
        this.handleCancelCreate();

        this.getDisciplines(this.state.filter);
    }

    handleClickEdit = (discipline) => {
        discipline.effectiveDate = moment(discipline.effectiveDate);

        this.setState({
            editDiscipline: {
                showModal: true,
                discipline: discipline
            }
        });
    }

    handleCancelEdit = () => {
        this.setState({
            editDiscipline: {
                showModal: false,
                discipline: null
            }
        });
    }

    handleEditSuccess = (discipline) => {
        let disciplines = this.state.disciplines.slice();
        let index = disciplines.findIndex(e => e.disciplineId === discipline.disciplineId);
        if (index >= 0) {
            disciplines[index] = discipline;
            this.setState({ disciplines: disciplines });
        }

        this.handleCancelEdit();
    }

    handleEditStatus = (disciplineId, isActive) => {
        let disciplines = this.state.disciplines.slice();
        let index = disciplines.findIndex(e => e.disciplineId === disciplineId);
        if (index >= 0) {
            disciplines[index].isActive = isActive;
        }

        this.setState({ disciplines: disciplines });
    }

    render() {
        return (
            <div className="row content-wrapper">
                <ToastLayout />
                <Breadcrumb>
                    <li className="current">{getResource('Discipline')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm
                        filter={this.state.filter}
                        groupOptions={this.groupOptions}
                        employeeOptions={this.employeeOptions}
                        onChange={this.handleFilterChange}
                        onSubmit={this.handleFilterSubmit}
                    />

                    <Table
                        data={this.state.disciplines}
                        isHost={this.isHost}
                        onCreate={this.handleClickCreate}
                        onEdit={this.handleClickEdit}
                        onEditStatus={this.handleEditStatus}
                    />
                </div>
                {
                    this.state.showCreateModal &&
                    <Create
                        employeeOptions={this.allEmployeeOptions}
                        showModal={this.state.showCreateModal}
                        onClose={this.handleCancelCreate}
                        onCreateSuccess={this.handleCreateSuccess}
                    />
                }
                {
                    this.state.editDiscipline.showModal &&
                    <Edit
                        discipline={this.state.editDiscipline.discipline}
                        showModal={this.state.editDiscipline.showModal}
                        onClose={this.handleCancelEdit}
                        onEditSuccess={this.handleEditSuccess}
                    />
                }
            </div>
        );
    }
}

ReactDOM.render(<Discipline />, document.getElementById("discipline-wrapper"));