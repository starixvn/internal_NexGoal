import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import SearchForm from './SearchForm';
import Table from './Table';

export default class PAFocus extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: null,
                groups: [],
                employee: null
            },
            periodOptions: [],
            groupOptions: [],
            memberOptions: [],
            paFocuses: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('pafocus/getPeriod')),
            axios(resolveClientUrl('pafocus/getGroup')),
            axios(resolveClientUrl('pafocus/getMember'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupResponse, memberResponse]) => {
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

                let memberOptions = [];
                memberResponse.data.map(item => {
                    memberOptions.push({
                        value: item.employeeId,
                        label: `${item.fullName} (${item.nickName})`
                    });
                });

                const filter = Object.assign({}, this.state.filter, {
                    period: periodOptions[0],
                    groups: groupOptions
                });

                this.getPAFocus(filter);

                this.setState({
                    filter: filter,
                    periodOptions: periodOptions,
                    groupOptions: groupOptions,
                    memberOptions: memberOptions
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
                groupIds: groupIds,
                employeeId: filter.employee ? filter.employee.value : ''
            }
        }).then(response => {
            this.setState({ paFocuses: response.data });
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
                    memberOptions={this.state.memberOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                />

                <Table data={this.state.paFocuses} />
            </Fragment>
        );
    }
}