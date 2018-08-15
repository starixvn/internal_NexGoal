import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import commonFunctions from '../../../../common-functions';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoyId: document.getElementById('EOYId').value,
            filter: {
                keyword: '',
                groupIds: '',
                roleId: '',
                isShowAll: false
            },
            groups: [],
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
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const data = response.data;

                this.setState({
                    groups: data,
                    filter: Object.assign({}, this.state.filter, { groupIds: data.map(g => g.groupId).toString() })
                });
            }).catch(commonFunctions.axiosError);
    }

    getNominatorSetting() {
        axios.get(commonFunctions.getRootUrl()
            + `EOY/GetNominatorSetting?eoyId=${this.state.eoyId}`
            + `&keyword=${this.state.filter.keyword}`
            + `&groupIds=${this.state.filter.groupIds}`
            + `&roleId=${this.state.filter.roleId}`
            + `&isShowAll=${this.state.filter.isShowAll}`)
            .then(response => {
                let data = response.data;
                data.map((e, index) => e.rowNum = index + 1);

                this.setState({ nominators: data });
            }).catch(commonFunctions.axiosError);
    }

    editNominatorSetting(nominator) {
        const setting = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqyId: this.state.eoyId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        });

        axios({
            url: commonFunctions.getRootUrl() + 'EOY/EditNominatorSetting',
            method: 'POST',
            data: setting,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
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
                    groups={this.state.groups}
                    handleFilterChange={this.handleFilterChange}
                    handleFilterSubmit={this.handleSubmit} />

                <Table isHost={this.state.isHost}
                    nominators={this.state.nominators}
                    editNominatorSetting={this.editNominatorSetting} />
            </Fragment>
        );
    }
}