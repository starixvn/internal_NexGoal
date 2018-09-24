import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';

import SearchForm from './SearchForm';
import Table from './Table';

export default class Review extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: null,
                groups: null
            },
            periodOptions: [],
            groupOptions: [],
            paRecords: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('review/getPeriods'))
            .then(response => {
                let periodOptions = response.data.map(period => {
                    return {
                        value: period.periodId,
                        label: period.name
                    };
                });
                let period = periodOptions.length > 0 ? periodOptions[0] : null;
                let nextState = Object.assign({}, this.state.filter, { period: period });

                if (period) {
                    axios.get(resolveClientUrl(`review/getGroups?periodId=${period.value}`))
                        .then(response => {
                            let groupOptions = response.data.map(group => {
                                return {
                                    value: group.groupId,
                                    label: group.name
                                };
                            });
                            nextState = Object.assign({}, nextState, { groups: groupOptions });

                            this.setState({
                                periodOptions: periodOptions,
                                groupOptions: groupOptions,
                                filter: nextState
                            });

                            this.getPARecords(nextState);
                        }).catch(axiosError);
                }
            }).catch(axiosError);
    }

    getGroups = (periodId) => {
        axios.get(resolveClientUrl(`review/getGroups?periodId=${periodId}`))
            .then(response => {
                let groupOptions = response.data.map(group => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                this.setState({
                    groupOptions: groupOptions,
                    filter: Object.assign({}, this.state.filter, { groups: groupOptions })
                });
            }).catch(axiosError);
    }

    getPARecords = (filter) => {
        axios.get(resolveClientUrl('review/getPARecords'), {
            params: {
                periodId: filter.period ? filter.period.value : 0,
                groupIds: filter.groups.map(group => { return group.value; }).join(',')
            }
        }).then(response => {
            this.setState({
                paRecords: response.data
            });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit = () => {
        const filter = this.state.filter;
        this.getPARecords(filter);
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Review</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <SearchForm filter={this.state.filter}
                            periodOptions={this.state.periodOptions}
                            groupOptions={this.state.groupOptions}
                            onFilterChange={this.handleFilterChange}
                            onSubmit={this.handleFilterSubmit}
                        />
                        <Table data={this.state.paRecords} />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<Review />, document.getElementById('review-wrapper'));