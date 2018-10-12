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
            focusEvaluationLevelOptions: [],
            periodOptions: [],
            groupOptions: [],
            periodSelected: null,
            pafocuses: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('pafocus/getPeriod')),
            axios(resolveClientUrl('pafocus/getGroup')),
            axios(resolveClientUrl('pafocus/getFocusEvaluationLevel'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupResponse, levelResponse]) => {
                const periodOptions = periodResponse.data.map((period) => {
                    return {
                        value: period.periodId,
                        label: period.name,
                        focusStartDate: period.focusStartDate,
                        focusEndDate: period.focusEndDate
                    };
                });

                const groupOptions = groupResponse.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                const filter = Object.assign({}, this.state.filter, {
                    period: periodOptions[0],
                    groups: groupOptions
                });

                this.getPAFocus(filter);

                this.setState({
                    filter: filter,
                    focusEvaluationLevelOptions: levelResponse.data,
                    periodOptions: periodOptions,
                    groupOptions: groupOptions
                });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    getPAFocus = (filter) => {
        const groupIds = filter.groups.map((group) => { return group.value; }).join(',');

        axios(resolveClientUrl('pafocus/getPAFocus'), {
            params: {
                periodId: filter.period.value,
                groupIds: groupIds
            }
        }).then(response => {
            this.setState({
                periodSelected: filter.period,
                pafocuses: response.data.paFocuses
            });
        }).catch(axiosError);
    }

    handleFilterSubmit = () => {
        let filter = this.state.filter;
        this.getPAFocus(filter);
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    periodOptions={this.state.periodOptions}
                    groupOptions={this.state.groupOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                />

                <ScatterChart
                    period={this.state.periodSelected}
                    focusEvaluationLevelOptions={this.state.focusEvaluationLevelOptions}
                    pafocuses={this.state.pafocuses}
                />
            </Fragment>
        );
    }
}