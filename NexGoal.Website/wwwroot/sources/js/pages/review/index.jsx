import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';

import SearchForm from './SearchForm';

export default class Review extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: null,
                groups: null
            },
            periodOptions: [],
            groupOptions: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('review/getPeriod'))
            .then(response => {
                let periodOptions = response.data.map(period => {
                    return {
                        value: period.periodId,
                        label: period.name
                    };
                });
                let period = periodOptions.length > 0 ? periodOptions[0] : null;

                if (period) {
                    this.getGroups(period.value);
                }

                this.setState({
                    periodOptions: periodOptions,
                    filter: Object.assign({}, this.state.filter, { period: period })
                });
            }).catch(axiosError);
    }

    getGroups = (periodId) => {
        axios.get(resolveClientUrl(`review/getGroup?periodId=${periodId}`))
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

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit = () => {
        const filter = this.state.filter;
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
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<Review />, document.getElementById('review-wrapper'));