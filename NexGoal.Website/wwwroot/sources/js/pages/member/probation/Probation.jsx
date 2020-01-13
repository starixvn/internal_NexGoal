import React, { Component } from 'react';
import axios from 'axios';
import { stringify } from 'qs';
import moment from 'moment';
import { toast } from 'react-toastify';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PostRequestHeaders } from '../../../constants';
import getResource from '../../../resourceHelper';
import Table from './Table';
import CommentModal from './CommentModal';

export default class Probation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationers: null,
            commentModal: {
                show: false,
                content: ''
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Probation/GetEvaluations'))
        ];

        Promise.all(promises)
            .then(([evaluationResponse]) => {
                evaluationResponse.data.forEach(probationer =>
                    probationer.settings.forEach(setting => {
                        setting.fromDate = moment(setting.fromDate);
                        setting.toDate = moment(setting.toDate);
                    })
                );

                this.setState({
                    probationers: evaluationResponse.data
                });
            }).catch(axiosError);
    }

    getProbationer = employeeId => {
        axios.get(resolveClientUrl('Probation/GetEvaluation'), {
            params: {
                employeeId
            }
        }).then(response => {
            const probationers = this.state.probationers.slice();
            const index = probationers.findIndex(p => p.employeeId === employeeId);

            probationers[index] = response.data;

            this.setState({ probationers });
        }).catch(axiosError);
    }

    handleConfirmedByReviewer = probationer => {
        axios({
            url: resolveClientUrl('Probation/ConfirmEvaluatorResult'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeId: probationer.employeeId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
                this.getProbationer(probationer.employeeId);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleShowCommentModal = (comment, processId) => this.setState({ commentModal: { show: true, content: comment, processId } })

    handleCloseCommentModal = () => this.setState({ commentModal: { show: false } })

    render() {
        return (
            <div className="probation-content">
                <Table tableData={this.state.probationers} onShowComment={this.handleShowCommentModal}
                    onConfirmedByReviewer={this.handleConfirmedByReviewer}
                />
                <CommentModal showModal={this.state.commentModal.show} content={this.state.commentModal.content}
                    processId={this.state.commentModal.processId}
                    onCloseModal={this.handleCloseCommentModal}
                />
            </div>
        );
    }
}