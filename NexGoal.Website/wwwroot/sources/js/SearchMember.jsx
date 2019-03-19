import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';
import getResource from './resourceHelper';

export default class SearchMember extends Component {
    constructor(props) {
        super(props);

        this.state = {
            hasMember: document.getElementById('employeeInfo-isHasMember').checked,
            showSearchbox: false,
            memberOptions: []
        };

        this.members = [];
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    getMemberOptions = () => {
        axios.get(resolveClientUrl('common/GetMyMember'))
            .then(response => {
                this.members = response.data;

                this.setState({
                    hasMember: this.members.length > 0
                });
            }).catch(axiosError);
    }

    handleClickOutside = event => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target) && this.state.showSearchbox && !this.inputRef.value) {
            this.setState({
                showSearchbox: false,
                memberOptions: []
            });
        }
    }

    handleChange = event => {
        const value = event.target.value.toLowerCase();

        if (value.length > 1) {
            let options = this.members.slice()
                .filter(member => member.nickName.toLowerCase().indexOf(value) >= 0 || member.fullName.toLowerCase().indexOf(value) >= 0)
                .map(member => {
                    return {
                        value: member.employeeId,
                        label: `${member.nickName} (${member.fullName})`
                    };
                });

            this.setState({ memberOptions: options });
        } else {
            this.setState({ memberOptions: [] });
        }
    }

    handleClick = () => {
        this.inputRef.focus();

        if (!this.state.showSearchbox) {
            this.setState({
                showSearchbox: true
            }, () => this.getMemberOptions());
        } else {
            this.setState({
                showSearchbox: false
            });
        }
    }

    handleSelect = event => {
        const memberId = event.target.attributes['memberid'].value;
        if (memberId) {
            window.location = resolveClientUrl(`member/details/${memberId}`);
        }
    }

    render() {
        const memberOptions = this.state.memberOptions;

        return (
            <Fragment>
                {
                    this.state.hasMember &&
                    <div ref={node => this.wrapperRef = node}
                        className={`search-member-container ${this.state.showSearchbox ? 'focused' : ''} ${memberOptions.length > 0 ? ' has-member' : ''}`}
                    >
                        <span className="icon-search" onClick={this.handleClick} />
                        <input ref={node => this.inputRef = node}
                            autoFocus
                            placeholder={getResource('NicknameOrFullName')}
                            onChange={this.handleChange}
                        />
                        {
                            memberOptions.length > 0 &&
                            <div className="member-list">
                                {
                                    memberOptions.map(member => {
                                        return (
                                            <div key={member.value}
                                                className="item"
                                                onClick={this.handleSelect}
                                                memberId={member.value}
                                            >
                                                {`${member.label}`}
                                            </div>
                                        );
                                    })
                                }
                            </div>
                        }
                    </div>
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<SearchMember />, document.getElementById('search-member-wrapper'));