import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../../common-functions';
import { PostRequestHeaders } from '../../../../constants';
import { nominationRoleOptions } from '../../common';
import getResource from '../../../../resourceHelper';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                keyword: '',
                groups: [],
                role: nominationRoleOptions[0],
                isShowAll: false
            },
            groupOptions: [],
            nominators: []
        };

        this.eoqId = document.getElementById('EOQId').value;
        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        axios(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                const groupOptions = response.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });

                const filter = Object.assign({}, this.state.filter, { groups: groupOptions });

                this.setState({
                    groupOptions: groupOptions,
                    filter: filter
                });

                this.getRoleSetting(filter);
            }).catch(axiosError);
    }

    getRoleSetting = filter => {
        const groupIds = this.state.filter.groups.map(group => group.value).join(',');

        axios.get(resolveClientUrl('nominationPeriod/GetRoleSetting'), {
            params: {
                nominationPeriodId: this.eoqId,
                keyword: filter.keyword,
                groupIds: groupIds,
                roleId: filter.role.value,
                isShowAll: filter.isShowAll
            }
        }).then(response => {
            this.setState({ nominators: response.data });
        }).catch(axiosError);
    }

    editNominatorSetting = nominator => {
        const setting = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            nominationPeriodId: this.eoqId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        });

        axios({
            url: resolveClientUrl('NominationPeriod/EditRoleSetting'),
            method: 'POST',
            data: setting,
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleSubmit = () => {
        this.getRoleSetting(this.state.filter);
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    groupOptions={this.state.groupOptions}
                    nominationRoleOptions={nominationRoleOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleSubmit}
                />

                <Table isHost={this.isHost}
                    nominators={this.state.nominators}
                    onEdit={this.editNominatorSetting}
                />
            </Fragment>
        );
    }
}