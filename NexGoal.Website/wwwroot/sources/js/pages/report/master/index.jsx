import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';

export default class MasterReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showModal: false,
            filter: {
                period: null,
                groups: [],
                employees: []
            },
            groupOptions: [],
            employeeOptions: [],
            periodOptions: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees')),
            axios.get(resolveClientUrl('Common/GetPeriodsForReport'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse, periodResponse]) => {
                const groupOptions = groupResponse.data.map(group => {
                    return {
                        id: group.groupId,
                        text: group.name,
                        checked: true
                    };
                });

                this.employees = employeeResponse.data;
                const employeeOptions = employeeResponse.data.map(item => {
                    return {
                        id: item.employeeId,
                        text: `${item.nickName} (${item.fullName})`,
                        checked:true
                    };
                });

                const periodOptions = periodResponse.data.map(x => ({ value: x.periodId, label: x.name }));

                const filter = {
                    ... this.state.filter,
                    groups: groupOptions,
                    period: periodOptions[0],
                    employees: employeeOptions
                };

                this.setState({ filter, employeeOptions, groupOptions, periodOptions });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            const employeeOptions = this.employees.filter(e => value.some(v => v.id === e.groupId))
                .map(e => ({
                    id: e.employeeId,
                    text: `${e.nickName} (${e.fullName})`,
                    checked: true
                }));

            this.setState({
                employeeOptions: employeeOptions,
                filter: {
                    ...this.state.filter,
                    [name]: value,
                    employees: employeeOptions
                }
            });
        } else {
            this.setState({
                filter: { ...this.state.filter, [name]: value }
            });
        }
    };

    render() {
        const { filter, periodOptions, groupOptions, employeeOptions } = this.state;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('MasterReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={filter}
                        groupOptions={groupOptions}
                        periodOptions={periodOptions}
                        employeeOptions={employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        exportUrl={`/MasterReport/Export?${stringify({
                            periodId: filter.period ? filter.period.value : null,
                            groupIds: filter.groups.map(x => x.id).toString(),
                            employeeIds: filter.employees.map(x => x.id).toString()
                        })}`}
                    />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<MasterReport />, document.getElementById('master-report-wrapper'));