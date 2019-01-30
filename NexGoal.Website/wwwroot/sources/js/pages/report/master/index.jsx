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
                employees: [],
            },
            groupOptions: [],
            employeeOptions: [],
            periodOptions: [],
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
                let groupOptions = groupResponse.data.map((group) => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                this.employees = employeeResponse.data;
                let employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                let periodOptions = periodResponse.data.map(x => ({ value: x.periodId, label: x.name }));

                let filter = {
                    ... this.state.filter,
                    ...{
                        groups: groupOptions,
                        period: periodOptions[0],
                        employees: employeeOptions
                    }
                };

                this.setState({ filter, employeeOptions, groupOptions, periodOptions });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            let employeeOptions = this.employees.filter(e => value.some(v => v.value === e.groupId))
                .map(e => ({
                    value: e.employeeId,
                    label: `${e.nickName} (${e.fullName})`
                }));

            this.setState({
                employeeOptions: employeeOptions,
                filter: {
                    ...this.state.filter,
                    ...{
                        [name]: value,
                        employees: employeeOptions
                    }
                }
            });
        } else {
            this.setState({
                filter: { ...this.state.filter, ...{ [name]: value } }
            });
        }
    }

    render() {
        const filter = this.state.filter;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('MasterReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        groupOptions={this.state.groupOptions}
                        periodOptions={this.state.periodOptions}
                        employeeOptions={this.state.employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        exportUrl={`MasterReport/Export?${stringify({
                            periodId: filter.period ? filter.period.value : null,
                            groupIds: filter.groups.map(x => x.value).toString(),
                            employeeIds: filter.employees.map(x => x.value).toString(),
                        })}`}
                    />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<MasterReport />, document.getElementById("master-report-wrapper"));