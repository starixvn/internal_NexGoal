import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './Table';
import { PerformanceProcess,NoPROptions } from '../../../constants';


export default class PRTracking extends Component {
    constructor(props) {
        super(props);

        const defaultNoPR = { value: null, label: '' };

        this.state = {
            filter: {
                groups: [],
                employees: [],
                noPR: null
            },
            groupOptions: [],
            employeeOptions: [],
            tableData: null,
            period: null
        };

        this.employees = [];
        this.noPROptions = [defaultNoPR, ...NoPROptions.map(ns => ({ value: ns.value, label: ns.text }))];

    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('PRTracking/GetGroupsAndMembers'))
        ];

        Promise.all(promises)
            .then(([response]) => {
                const groupOptions = response.data.groups.map(group => {
                    return {
                        id: group.groupId,
                        text: group.name,
                        checked: true
                    };
                });

                this.employees = response.data.employees;

                const employeeOptions = response.data.employees.map(item => {
                    return {
                        id: item.employeeId,
                        text: `${item.nickName} (${item.fullName})`,
                        checked: true
                    };
                });

                const filter = {
                    ... this.state.filter,
                    groups: groupOptions,
                    employees: employeeOptions
                };

                this.setState({ filter, employeeOptions, groupOptions }, this.getPRTracking);
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            const employeeChoosens = this.employees.filter(e => value.some(v => v.id === e.groupId))
                .map(e => ({
                    id: e.employeeId,
                    text: `${e.nickName} (${e.fullName})`,
                    checked: true
                }));

            this.setState({
                employeeOptions: employeeChoosens,
                filter: {
                    ...this.state.filter,
                    [name]: value,
                    employees: employeeChoosens
                }
            });
        } else {
            this.setState({
                filter: {
                    ...this.state.filter,
                    [name]: value
                }
            });
        }
    };

    getPRTracking = () => {
        const filter = this.state.filter;

        axios.get(resolveClientUrl('PRTracking/Get'), {
            params: {
                noPR: filter.noPR ? filter.noPR.value : null,
                employeeIds: filter.employees.map(x => x.id).toString()
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
        const { filter, period, groupOptions, employeeOptions, tableData } = this.state;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PRReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    {
                        this.employees.length > 1 && groupOptions.length > 0 &&
                        <SearchForm filter={filter}
                            groupOptions={groupOptions}
                            employeeOptions={employeeOptions}
                            noPROptions={this.noPROptions}
                            onFilterChange={this.handleFilterChange}
                            onSubmit={this.getPRTracking}
                        />
                    }

                    <Table tableData={tableData}
                        period={period}
                    />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<PRTracking />, document.getElementById('pr-tracking-wrapper'));