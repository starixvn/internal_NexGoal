import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import SearchForm from './SearchForm';
import ScatterChart from './ScatterChart';

export default class Chart extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: null,
                groups: []
            },
            pafocuses: []
        };

        this.periodOptions = [];
        this.groupOptions = [];
        this.focusEvaluationLevelOptions = [];
        this.periodSelected = null;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('pafocus/getPeriod')),
            axios(resolveClientUrl('pafocus/getGroup')),
            axios(resolveClientUrl('pafocus/getFocusEvaluationLevel'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupResponse, levelResponse]) => {
                const periodOptions = periodResponse.data.map(period => {
                    return {
                        value: period.periodId,
                        label: period.name,
                        focusStartDate: period.focusStartDate,
                        focusEndDate: period.focusEndDate
                    };
                });

                const groupOptions = groupResponse.data.map(group => {
                    return { id: group.groupId, text: group.name, checked: true };
                });

                const filter = {
                    ...this.state.filter,
                    period: periodOptions[0],
                    groups: groupOptions
                };

                this.getPAFocus(filter);

                this.periodOptions = periodOptions;
                this.groupOptions = groupOptions;
                this.focusEvaluationLevelOptions = levelResponse.data;
                this.setState({ filter: filter });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: { ...this.state.filter, [name]: value }
        });
    };

    getPAFocus = filter => {
        const periodId = filter.period ? filter.period.value : null;
        const groupIds = filter.groups.map(group => group.id).join(',');

        axios(resolveClientUrl('pafocus/getPAFocus'), {
            params: {
                periodId: periodId,
                groupIds: groupIds
            }
        }).then(response => {
            this.periodSelected = filter.period;
            this.setState({
                pafocuses: response.data.paFocuses
            });
        }).catch(axiosError);
    };

    handleFilterSubmit = () => {
        const filter = this.state.filter;
        this.getPAFocus(filter);
    };

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    periodOptions={this.periodOptions}
                    groupOptions={this.groupOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                />

                <ScatterChart
                    period={this.periodSelected}
                    focusEvaluationLevelOptions={this.focusEvaluationLevelOptions}
                    pafocuses={this.state.pafocuses}
                />
            </Fragment>
        );
    }
}