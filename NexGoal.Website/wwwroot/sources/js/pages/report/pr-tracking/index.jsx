import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './Table';
import { PerformanceProcess } from '../../../constants';

export default class PRTracking extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                groups: [],
                employees: [],
                noPR: false
            },
            groupOptions: [],
            employeeOptions: [],
            tableData: null,
            period: null
        };

        this.employees = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('PRTracking/GetGroupsAndMembers'))
        ];

        Promise.all(promises)
            .then(([response]) => {
                const groupOptions = response.data.groups.map(group => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                this.employees = response.data.employees;

                const employeeOptions = response.data.employees.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                const filter = {
                    ... this.state.filter,
                    ...{
                        groups: groupOptions,
                        employees: employeeOptions
                    }
                };

                this.setState({ filter, employeeOptions, groupOptions }, this.getPRTracking);
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            const employeeChoosens = this.employees.filter(e => value.some(v => v.value === e.groupId))
                .map(e => ({
                    value: e.employeeId,
                    label: `${e.nickName} (${e.fullName})`
                }));

            this.setState({
                employeeOptions: employeeChoosens,
                filter: {
                    ...this.state.filter,
                    ...{
                        [name]: value,
                        employees: employeeChoosens
                    }
                }
            });
        } else {
            this.setState({
                filter: {
                ...this.state.filter,
                ...{
                    [name]: value }
                }
            });
        }
    };

    getPRTracking = () => {
        const filter = this.state.filter;

        axios.get(resolveClientUrl('PRTracking/Get'), {
            params: {
                noPR: filter.noPR,
                employeeIds: filter.employees.map(x => x.value).toString()
            }
        }).then(response => {
            this.setState({
                tableData: response.data.employees.reduce(function (groups, item) {
                    item.selfPRQuantity = item.details.filter(d => d.roleId === PerformanceProcess.Self).reduce((current, i) => current + i.quantity, 0);
                    item.pmPRQuantity = item.details.filter(d => d.roleId === PerformanceProcess.ProjectManager).reduce((current, i) => current + i.quantity, 0);
                    item.evaluatorPRQuantity = item.details.filter(d => d.roleId === PerformanceProcess.Evaluator).reduce((current, i) => current + i.quantity, 0);
                    item.reviewerPRQuantity = item.details.filter(d => d.roleId === PerformanceProcess.Reviewer).reduce((current, i) => current + i.quantity, 0);

                    let group = groups.find(e => e.groupId === item.groupId);

                    if (!group) {
                        group = {
                            groupId: item.groupId,
                            groupName: item.groupName,
                            employees: [item]
                        };

                        groups.push(group);
                    } else {
                        group.employees.push(item);
                    }

                    return groups;
                }, []),
                period: response.data.period
            });
        }).catch(axiosError);
    };
    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PRReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    {this.employees.length > 1 && this.state.groupOptions.length > 0 &&
                        <SearchForm filter={this.state.filter}
                            groupOptions={this.state.groupOptions}
                            employeeOptions={this.state.employeeOptions}
                            onFilterChange={this.handleFilterChange}
                            onSubmit={this.getPRTracking}
                        />
                    }

                    <Table tableData={this.state.tableData}
                        period={this.state.period}
                    />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<PRTracking />, document.getElementById('pr-tracking-wrapper'));