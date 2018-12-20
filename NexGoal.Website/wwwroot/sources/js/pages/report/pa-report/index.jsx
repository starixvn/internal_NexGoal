import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import Filter from './filter';
import Table from './table';

export default class App extends Component {
    constructor(props) {
        super(props);
        this.state = {
            periodOptions: [],
            groupOptions: [],
            employeeOptions: [],
            filter: {
                periods: [],
                groups: [],
                employee: null
            },
            paPeriodRecords: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('Common/GetMyGroup')),
            axios(resolveClientUrl('Common/GetMyMember'))
        ];

        Promise.all(promises)
            .then(([periodResponse, myGroupResponse, employeeResponse]) => {
                let periodOptions = periodResponse.data.map((period) => {
                    return {
                        label: period.name,
                        value: period.periodId
                    };
                });

                let groupOptions = myGroupResponse.data.map((group) => {
                    return {
                        label: group.name,
                        value: group.groupId
                    };
                });

                let employeeOptions = employeeResponse.data.map((employee) => {
                    return {
                        label: `${employee.nickName} (${employee.fullName})`,
                        value: employee.employeeId,
                        groupName: employee.groupName
                    };
                });

                this.setState({
                    periodOptions: periodOptions,
                    groupOptions: groupOptions,
                    employeeOptions: employeeOptions,
                    filter: {
                        periods: [periodOptions[0]],
                        groups: groupOptions
                    }
                }, () => this.getPARecords());
            }).catch(axiosError);
    }

    handleFilterChange = (filterName, values) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [filterName]: values })
        });
    }

    handleFilterSubmit = () => {
        this.getPARecords();
    }

    getPARecords = () => {
        const filter = this.state.filter;
        axios.get(resolveClientUrl('Report/GetPARecords'), {
            params: {
                periodIds: filter.periods.map((period) => { return period.value; }).join(','),
                groupIds: filter.groups.map(group => { return group.value; }).join(','),
                employeeId: filter.employee ? filter.employee.value : null
            }
        }).then(response => {
            this.setState({
                paPeriodRecords: response.data
            });
        }).catch(axiosError);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('EvaluationPeriodManagement')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Filter periodOptions={this.state.periodOptions}
                        groupOptions={this.state.groupOptions}
                        employeeOptions={this.state.employeeOptions}
                        filter={this.state.filter}
                        onFilterChange={this.handleFilterChange}
                        onSubmit={this.handleFilterSubmit}
                    />
                    <Table periodRecords={this.state.paPeriodRecords} />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('pa-report'));