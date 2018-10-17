import React, { Component } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';

export default class NominationResult extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominations: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('home/GetNominationResult'))
            .then(response => {
                this.setState({
                    nominations: response.data
                });
            }).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.state.nominations) !== JSON.stringify(nextState.nominations)) {
            return true;
        }

        return false;
    }

    getCssClass = (statusId) => {
        let cssClass = '';

        switch (statusId) {
            case NominationStatuses[0].value:
                cssClass = 'status-nomination';
                break;

            case NominationStatuses[1].value:
                cssClass = 'status-shortlisted';
                break;

            case NominationStatuses[2].value:
                cssClass = 'status-winner';
                break;
        }

        return cssClass;
    }

    render() {
        const nominations = this.state.nominations;

        return (
            <div className="nomination">
                <h5>Nomination</h5>
                {
                    nominations.map(nomination => {
                        return (
                            <div key={`nomination${nomination.eoqyId}`}>
                                {nomination.period.title} - <span className={this.getCssClass(nomination.statusId)}>{NominationStatuses.find(x => x.value === nomination.statusId).text}</span>
                            </div>
                        );
                    })
                }
            </div>
        );
    }
}