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
            filter: {
                periods: [],
                groups: [],
                employee: null
            },
            paPeriodRecords: []
        };

        this.periodOptions = [];
        this.groupOptions = [];
        this.employeeOptions = [];
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
                this.periodOptions = periodOptions;
                this.groupOptions = groupOptions;
                this.employeeOptions = employeeOptions;

                this.setState({
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
        }, () => {
            if (filterName === 'groups') {
                const groupIds = values.length > 0 ? values.map(group => group.value).join(',') : '-1';
                this.getEmployees(groupIds);
            }
        });
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

    handleFilterSubmit = () => {
        this.getPARecords();
    }

    handleExportExcel = () => {
        const filter = this.state.filter;

        axios({
            url: resolveClientUrl('report/ExportPAReport'),
            params: {
                periodIds: filter.periods.map((period) => { return period.value; }).join(','),
                groupIds: filter.groups.map(group => { return group.value; }).join(','),
                employeeId: filter.employee ? filter.employee.value : null
            },
            method: 'get',
            responseType: 'blob'
        }).then((response) => {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            let link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `PA Report.xlsx`);
            document.body.appendChild(link);
            link.click();
        }).catch(axiosError);
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
                    <li className="current">{getResource('PerformanceAppraisalReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Filter periodOptions={this.periodOptions}
                        groupOptions={this.groupOptions}
                        employeeOptions={this.employeeOptions}
                        filter={this.state.filter}
                        onFilterChange={this.handleFilterChange}
                        onSubmit={this.handleFilterSubmit}
                        exportExcel={this.handleExportExcel}
                    />
                    <Table periodRecords={this.state.paPeriodRecords} exportPDF={this.handleExportPDF} />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('pa-report'));