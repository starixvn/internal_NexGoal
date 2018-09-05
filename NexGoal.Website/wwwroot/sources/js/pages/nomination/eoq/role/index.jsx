import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../../common-functions';
import { nominationRoleOptions } from '../../common';
import getResource from '../../../../resourceHelper';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoqId: document.getElementById('EOQId').value,
            filter: {
                keyword: '',
                groups: [],
                role: nominationRoleOptions[0],
                isShowAll: false
            },
            groupOptions: [],
            nominators: [],
            isHost: document.getElementById('IsHost').checked
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.editNominatorSetting = this.editNominatorSetting.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        this.getGroups();
        this.getNominatorSetting();
    }

    getGroups() {
        axios(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                const groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                this.setState({
                    groupOptions: groupOptions,
                    filter: Object.assign({}, this.state.filter, { groups: groupOptions })
                });
            }).catch(axiosError);
    }

    getNominatorSetting() {
        const filter = this.state.filter;
        const groupIds = this.state.filter.groups.map((group) => { return group.value; }).join(',');

        axios.get(resolveClientUrl('EOQ/GetNominatorSetting'), {
            params: {
                eoqId: this.state.eoqId,
                keyword: filter.keyword,
                groupIds: groupIds,
                roleId: filter.role.roleId,
                isShowAll: filter.isShowAll
            }
        }).then(response => {
            const data = response.data;
            data.map((e, index) => e.rowNum = index + 1);

            this.setState({ nominators: data });
        }).catch(axiosError);
    }

    editNominatorSetting(nominator) {
        const setting = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqyId: this.state.eoqId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        });

        axios({
            url: resolveClientUrl('EOQ/EditNominatorSetting'),
            method: 'POST',
            data: setting,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdateSuccessfully'));

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleFilterChange(filter) {
        this.setState({ filter: filter });
    }

    handleSubmit() {
        this.getNominatorSetting();
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    groupOptions={this.state.groupOptions}
                    nominationRoleOptions={nominationRoleOptions}
                    handleFilterChange={this.handleFilterChange}
                    handleFilterSubmit={this.handleSubmit}
                />

                <Table isHost={this.state.isHost}
                    nominators={this.state.nominators}
                    editNominatorSetting={this.editNominatorSetting}
                />
            </Fragment>
        );
    }
}