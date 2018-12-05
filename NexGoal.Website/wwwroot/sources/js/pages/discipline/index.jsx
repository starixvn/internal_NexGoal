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
import AddEditDiscipline from './EditDiscipline';

export default class DisciplineManagement extends Component {
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
            data: [],
            groupOptions: [],
            employeeOptions: [],
            isHost: document.getElementById('IsHost').checked,
            showModal: false
        };
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
                        label: `${item.fullName} (${item.nickName})`
                    };
                });

                this.setState({
                    groupOptions: groupOptions,
                    employeeOptions: employeeOptions
                });

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
                        label: `${item.fullName} (${item.nickName})`
                    };
                });
                let filter = this.state.filter;

                if (employeeOptions.indexOf(filter.employee) < 0) {
                    filter = Object.assign({}, this.state.filter, { employee: null });
                }

                this.setState({
                    filter: filter,
                    employeeOptions: employeeOptions
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
            this.setState({ data: response.data });
        }).catch(axiosError);
    }

    onUpdateStatus = (disciplineId, isActive) => {
        let data = this.state.data.slice();
        let index = data.findIndex(e => e.disciplineId === disciplineId);
        if (index >= 0) {
            data[index].isActive = isActive;
        }

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
                        onChange={this.handleFilterChange}
                        onSubmit={this.handleFilterSubmit}
                    />

                    <Table data={this.state.data}
                        onUpdateStatus={this.onUpdateStatus}
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