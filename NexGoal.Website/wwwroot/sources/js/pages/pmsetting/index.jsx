import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './Table';

export default class PMSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                pms: []
            },
            pmOptions: [],
            pms: [],
            tableData: []
        }
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('common/GetEmployees'), {
                params: {
                    groupIds: 13
                }
            }),
        ];

        Promise.all(promises)
            .then(([employeeResponse]) => {
                let pmOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                let filter = {
                    pms: pmOptions
                };

                this.setState({ filter: filter, pmOptions: pmOptions, pms: employeeResponse.data }, this.getPMSetting);
            }).catch(axiosError);
    }

    getPMSetting = () => {
        let pms = this.state.pms.filter(e => this.state.filter.pms.some(p => p.value === e.employeeId));

        this.setState({
            tableData: pms.map(pm => ({
                pm: pm,
                employees: []
            }))
        });
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: { ...this.state.filter, ...{ [name]: value } }
        });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PMSetting')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        pmOptions={this.state.pmOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.getPMSetting}
                    />
                    <Table tableData={this.state.tableData} />
                </div>
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<PMSetting />, document.getElementById("pm-setting-wrapper"));